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

        return "";//Se passar pelas duas retorna vazio, e segue...
    }

  
}
