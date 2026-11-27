public class PurchaseMenu
{
    public PurchaseModule module { get; private set; } = new PurchaseModule();

    public void Show()
    {
        while (true)
        {
            Console.WriteLine("=== COMPRAS ===");
            Console.WriteLine("1 - Cadastrar compra");
            Console.WriteLine("2 - Localizar compra");
            Console.WriteLine("3 - Excluir compra");
            Console.WriteLine("4 - Navegar pelas compras");
            Console.WriteLine("0 - Voltar");

            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    RegisterPurchase();
                    break;
                case 2:
                    SearchPurchase();
                    break;
                case 3:
                    DeletePurchase();
                    break;
                case 4:
                    NavigatePurchases();
                    break;
                case 0:
                    return;
                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }
    //registro de compra
    public void RegisterPurchase()
    {
        //input fornecedor
        Console.Write("CNPJ do fornecedor (só números): ");
        string cnpj = Console.ReadLine();

        //validação de supplier
        if (!Supplier.ValidateCNPJ(cnpj))
        {
            Console.WriteLine("CNPJ inválido.");
            return;
        }
        
        SupplierFiles supplierFiles = new SupplierFiles();
        Supplier supplier = supplierFiles.FindCNPJ(cnpj);

        //se for vazio: forncedor nao encontrado
        if (supplier == null)
        {
            Console.WriteLine("Fornecedor não cadastrado.");
            return;
        }

        //vai verificar o status do supplier na lista de bloqueados
        bool isBlocked = supplierFiles.FindBlockedSupplier(cnpj);
        string error = module.ValidateSupplier(isBlocked, supplier.OpeningDate);

        if (error != "")
        {
            Console.WriteLine(error);
            return;
        }

        Console.WriteLine("Razão social: " + supplier.CorporateName);
        Console.WriteLine("Data de abertura: " + supplier.OpeningDate.ToString("dd/MM/yyyy"));

        //Se respota diferente de S, ele fecha
        Console.Write("Confirma o fornecedor? (S/N): ");
        if (Console.ReadLine().ToUpper() != "S")
        {
            Console.WriteLine("Compra cancelada.");
            return;
        }

        //cria compra
        Purchase purchase = module.StartPurchase(cnpj);

        //cria lista para manipulação de itens
        List<PurchaseItem> items = new List<PurchaseItem>();

        
        //enquanto o usuario desejar continuar e o número de itens for menor que 3 ele vai executar este laço
        bool addMore = true;
        while (addMore && items.Count < 3)
        {

            //input código da materia prima
            Console.Write("Código da matéria-prima (MP0000): ");
            string rawId = Console.ReadLine();

            decimal quantity;
            decimal unitValue;

            //input Quatidade e valorUnitario
            try
            {
                Console.Write("Quantidade: ");
                quantity = decimal.Parse(Console.ReadLine());

                Console.Write("Valor unitário: ");
                unitValue = decimal.Parse(Console.ReadLine());
            }
            catch (FormatException)
            {
                Console.WriteLine("Digite apenas números (use vírgula para centavos).");
                continue;
            }

            error = module.ValidateItem(purchase, items.Count, rawId, quantity, unitValue);

            if (error == "")
                items.Add(module.AddItem(purchase, rawId, quantity, unitValue));
            else
                Console.WriteLine(error);

            if (items.Count < 3)
            {
                Console.Write("Adicionar outro item? (S/N): ");
                addMore = Console.ReadLine().ToUpper() == "S";
            }
        }

        error = module.FinishPurchase(purchase, items);

        if (error == "")
            Console.WriteLine("Compra " + purchase.Id + " gravada. Total: " + purchase.TotalValue.ToString("F2"));
        else
            Console.WriteLine(error);
    }
    //Busca de Compra
    public void SearchPurchase()
    {
        //recebe o input do id digitado pelo usuário
        Console.Write("Id da compra: ");
        string id = Console.ReadLine().PadLeft(5, '0');

        //objetos para manipular os arquivos de compra e itemCompra
        PurchaseFile purchaseFile = new PurchaseFile();
        PurchaseItemFile itemFile = new PurchaseItemFile();

        //Se a validação de compra ou itemCompra retornar false, ele sai do método
        if (!purchaseFile.FileValidation() || !itemFile.FileValidation())
            return;

        //status de localização da compra (Por padrão começa em false)
        bool found = false;
        string line;
        StreamReader reader = new StreamReader(purchaseFile.path + purchaseFile.file);

        while ((line = reader.ReadLine()) != null)
        {
            if (line.Substring(0, 5) == id)
            {
                found = true;

                string day = line.Substring(5, 2);
                string month = line.Substring(7, 2);
                string year = line.Substring(9, 4);
                string cnpj = line.Substring(13, 14);
                decimal total = decimal.Parse(line.Substring(27, 7)) / 100;

                Console.WriteLine("Compra: " + id);
                Console.WriteLine("Data: " + day + "/" + month + "/" + year);
                Console.WriteLine("Fornecedor (CNPJ): " + cnpj);
                Console.WriteLine("Total: " + total.ToString("F2"));
            }
        }
        reader.Close();

        if (!found)
        {
            Console.WriteLine("Compra não encontrada.");
            return;
        }

        Console.WriteLine("Itens:");
        reader = new StreamReader(itemFile.path + itemFile.file);
        while ((line = reader.ReadLine()) != null)
        {
            if (line.Substring(0, 5) == id)
            {
                string rawId = line.Substring(13, 6);
                decimal quantity = decimal.Parse(line.Substring(19, 5)) / 100;
                decimal unitValue = decimal.Parse(line.Substring(24, 5)) / 100;
                decimal itemTotal = decimal.Parse(line.Substring(29, 6)) / 100;

                Console.WriteLine(rawId + " | Qtd: " + quantity.ToString("F2") + " | Unit: " + unitValue.ToString("F2") + " | Total: " + itemTotal.ToString("F2"));
            }
        }
        reader.Close();
    }
    //excluir uma compra
    public void DeletePurchase()
    {

        //input do id desejado
        Console.Write("Id da compra a excluir: ");
        string id = Console.ReadLine().PadLeft(5, '0');
        //confirmação
        Console.Write("Excluir a compra " + id + " e todos os seus itens? (S/N): ");
        string answer = Console.ReadLine();

        if (answer.ToUpper() != "S")
        {
            Console.WriteLine("Exclusão cancelada.");
            return;
        }

        PurchaseFile purchaseFile = new PurchaseFile();
        PurchaseItemFile itemFile = new PurchaseItemFile();

        //se um dos arquivos nao estiver ok, ele vai sair
        if (!purchaseFile.FileValidation() || !itemFile.FileValidation())
            return;

        
        bool found = RemoveById(purchaseFile.path + purchaseFile.file, id);
        RemoveById(itemFile.path + itemFile.file, id);

        if (found)
            Console.WriteLine("Compra " + id + " e seus itens foram excluídos.");
        else
            Console.WriteLine("Compra não encontrada.");
    }

    private bool RemoveById(string filePath, string id)
    {
        //le o arquivo iteiro e passa para lista
        List<string> lines = new List<string>(File.ReadAllLines(filePath));
        //lista que os arquivos mantidos ficaram
        List<string> kept = new List<string>();

        bool found = false;

        //percorre a lista para verificar se o id escolhido esta
        foreach (string line in lines)
        {
            if (line.Substring(0, 5) == id)
                found = true;
            else
                kept.Add(line);
        }
        //grava no arquvio a lista apenas com as linhas que foram escritas (A com id selecionado nao estará aqui)
        File.WriteAllLines(filePath, kept);
        return found;
    }
    //Navegar pelas compras
    public void NavigatePurchases()
    {
        //Ainda vou fazer, calma!
    }
}