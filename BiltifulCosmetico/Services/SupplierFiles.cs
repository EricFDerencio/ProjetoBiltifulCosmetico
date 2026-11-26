public class SupplierFiles
{
    private string path = Environment.GetEnvironmentVariable("USERPROFILE") + @"\biltiful\";
    private string supplierFile = "supplier.dat";
    private string blockedSuppliers = "blockeds.dat";

    private string suppliersPath;
    private string blockedsPath;


    public SupplierFiles()
    {
        Directory.CreateDirectory(path);
        suppliersPath = Path.Combine(path, supplierFile);    // passa os caminhos dos arquivos que desejamos
        blockedsPath = Path.Combine(path, blockedSuppliers);
    }

    public Supplier FindCNPJ(string cnpj)   // busca fornecedor pelo cnpj e retorna seus dados (ou null, se nao encontrar)
    {
        if (!File.Exists(suppliersPath))
            return null;
        using (StreamReader reader = new StreamReader(suppliersPath))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string cnpjFromFile = line.Substring(0, 14);

                if (cnpjFromFile == cnpj)
                {
                    string corporateName = line.Substring(14, 50);
                    DateOnly openingDate = DateOnly.ParseExact(line.Substring(64, 8), "ddMMyyyy", null);
                    DateOnly lastPurchase = DateOnly.ParseExact(line.Substring(72, 8), "ddMMyyyy", null);
                    DateOnly registrationDate = DateOnly.ParseExact(line.Substring(80, 8), "ddMMyyyy", null);
                    ESituation situation = (ESituation)line[88];

                    return new Supplier(cnpjFromFile, corporateName, openingDate, lastPurchase, registrationDate, situation);
                }
            }
        }
        return null;
    }

    public bool RegisterSupplier(Supplier newSupplier) // confere se o cnpj ainda nao foi cadastrado e, se nao tiver sido, faz o cadastro
    {
        if (FindCNPJ(newSupplier.CNPJ) != null)
        {
            Console.WriteLine("Este CNPJ já foi cadastrado.");
            return false;
        }

        using (StreamWriter writer = new StreamWriter(suppliersPath, true))
        {
            writer.WriteLine(newSupplier.ToFileString());
        }
        return true;
    }

    public bool EditSupplier(string cnpj, Supplier updateSupplier) // localiza fornecedor usando lista e edita as infos
    {
        if (!File.Exists(suppliersPath))
        {
            Console.WriteLine("Este fornecedor ainda não foi cadastrado, não é possível editá-lo.");
            return false;
        }

        List<string> suppliers = File.ReadAllLines(suppliersPath).ToList();

        int supplierIndex = suppliers.FindIndex(line => line.Substring(0, 14) == cnpj);

        if (supplierIndex == -1) // findIndex retorna -1 se nao encontrar o cnpj que queremos editar
        {
            Console.WriteLine("Fornecedor não encontrado.");
            return false;
        }
        suppliers[supplierIndex] = updateSupplier.ToFileString();

        File.WriteAllLines(suppliersPath, suppliers);
        return true;

    }

    public bool BlockSupplier(string cnpj) // verifica se o cnpj nao está na lista de bloqueados, e se nao tiver, adiciona
    {
        if (FindCNPJ(cnpj) == null)
        {
            Console.WriteLine("Este fornecedor não está cadastrado.");
            return false;
        }

        if (FindBlockedSupplier(cnpj))
        {
            Console.WriteLine("Este fornecedor já foi bloqueado anteriormente.");
            return false;
        }
        using (StreamWriter writer = new StreamWriter(blockedsPath, true))
        {
            writer.WriteLine(cnpj);
        }

        return true;
    }

    public bool FindBlockedSupplier(string cnpj) // percorre o arquivo de bloqueados e tenta encontrar o cnpj passado como parametro
    {
        if (!File.Exists(blockedsPath))
        {
            Console.WriteLine("Fornecedor não está bloqueado.");
            return false;
        }
        using (StreamReader reader = new StreamReader(blockedsPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line == cnpj)
                {
                    Console.WriteLine("Fornecedor está bloqueado.");
                    return true;
                }

            }
        }
        return false;
    }

    public bool UnlockSupplier(string cnpj) // verifica se o cnpj está na lista de bloqueados, e se tiver, remove
    {
        if (!File.Exists(blockedsPath))
        {
            Console.WriteLine("Fornecedor não está bloqueado.");
            return false;
        }
        List<string> blockeds = File.ReadAllLines(blockedsPath).ToList();

        if (!blockeds.Remove(cnpj))
        {
            Console.WriteLine("Fornecedor não está bloqueado.");
            return false;
        }
        File.WriteAllLines(blockedsPath, blockeds);
        return true;
    }
}


