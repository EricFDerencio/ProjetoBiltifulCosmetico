public class PurchaseModule
{
    private PurchaseFile FileForPurchase = new PurchaseFile();
    private PurchaseItemFile FileForItem = new PurchaseItemFile();

    //Compra inicial
    public Purchase StartPurchase(string cnpj)
    {
        int nextId = FileForPurchase.GetNextId();
        Purchase p = new Purchase(nextId, cnpj);
        return p;
    }

    //validação para ver se o fornecedor esta bloqueado ou tem menos de 6 meses de abertura
    public string ValidateSupplier(bool supplierIsBlocked, DateTime supplierOpeningDate)
    {
        if (supplierIsBlocked)
            return "Fornecedor bloqueado. Compra não permitida.";

        if (supplierOpeningDate.AddMonths(6) > DateTime.Today) //Se a data de abertura(+6) passar da data atual, ele não vai permitir
            return "Fornecedor com menos de 6 meses de abertura.";

        return "";
    }

    //validar se a matériaPrima existe e está com o status ativo
    public string ValidateRawMaterial(string rawMaterialId)
    {
        List<RawMaterial> materials = RawMaterialFile.LoadRawMaterials();

        foreach (RawMaterial material in materials)
        {
            if (material.Id == rawMaterialId)
            {
                if (material.Situation == ESituation.Inactive)
                    return "Matéria-prima inativa. Compra não permitida.";

                return "";
            }
        }

        return "Matéria-prima não encontrada.";
    }

    //cria o item e soma o valor dele no valor total da compra
    public PurchaseItem AddItem(Purchase purchase, string rawMaterialId, decimal quantity, decimal unitValue)
    {
        PurchaseItem item = new PurchaseItem(purchase.Id, purchase.PurchaseDate, rawMaterialId, quantity, unitValue);

        purchase.AddItemValue(item.ItemTotal);

        return item;
    }

    //método para salvar nos arquivos de Compra e itemCompra
    public string FinishPurchase(Purchase purchase, List<PurchaseItem> items)
    {
        if (items.Count == 0)
            return "A compra não possui itens. Nada foi salvo.";

        FileForPurchase.Save(purchase);

        foreach (PurchaseItem item in items)
        {
            FileForItem.Save(item);
        }

        return "";
    }


    //metodo para listar as compras
    public void ListPurchases()
    {
        //verifica se tem um arquivo
        if (!FileForPurchase.FileValidation())
            return;

        StreamReader reader = new StreamReader(FileForPurchase.path + FileForPurchase.file);

        string line;
        int count = 0;

        while ((line = reader.ReadLine()) != null)
        {
        
            int id = int.Parse(line.Substring(0, 5));
            string day = line.Substring(5, 2);
            string month = line.Substring(7, 2);
            string year = line.Substring(9, 4);
            string cnpj = line.Substring(13, 14);
            decimal total = decimal.Parse(line.Substring(27, 7)) / 100;

            Console.WriteLine($"ID: {id} | Data: {day}/{month}/{year} | CNPJ: {cnpj} | Total: {total:F2}");
            count++;
        }

        reader.Close();

        if (count == 0)
            Console.WriteLine("Nenhuma compra cadastrada.");
    }
}
