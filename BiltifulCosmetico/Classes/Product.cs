using System.Globalization;

public class Product
{
    public string Barcode { get; private set; }
    public string Name { get; private set; }
    public decimal SalePrice { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public DateTime LastSaleDate { get; private set; }
    public EStatus Status { get; private set; }

    public Product(List<Product> products)
    {
        Name = SetName();
        Barcode = SetGenerateBarcode(products);
        SalePrice = SetSalePrice();
        RegistrationDate = SetCurrentDate();
        LastSaleDate = SetCurrentDate();
        Status = EStatus.Ativo;
    }

    private Product(string barcode, string name, decimal salePrice, DateTime registrationDate, DateTime lastSaleDate, EStatus status)
    {
        Name = name;
        Barcode = barcode;
        SalePrice = salePrice;
        RegistrationDate = registrationDate;
        LastSaleDate = lastSaleDate;
        Status = status;
    }

    public string SetGenerateBarcode(List<Product> products)
    {
        int[] vector = new int[13];
        //requisito começar com 789
        vector[0] = 7;
        vector[1] = 8;
        vector[2] = 9;

        bool invalidCode = true;
        int sumWeights = 0;
        int remainderOfDivisionSum = 0;
        string readyBarcode = string.Empty;
        Random random = new Random();


        while (invalidCode)
        {
            sumWeights = 0;
            remainderOfDivisionSum = 0;

            //percorre o vetor do código de barras e preenche
            for (int i = 3; i <= 11; i++)
            {

                vector[i] = random.Next(0, 10);
            }

            //percorre o vetor do código somando à variável de acordo com o peso
            for (int i = 1; i <= 12; i++)
            {
                if (i % 2 != 0)
                {
                    sumWeights += (vector[i - 1] * 1);
                }
                else
                {
                    sumWeights += (vector[i - 1] * 3);
                }
            }

            //cálculo do último digito verificador
            remainderOfDivisionSum = sumWeights % 10;
            if (remainderOfDivisionSum == 0)
            {
                vector[12] = 0;
            }
            else
            {
                vector[12] = 10 - remainderOfDivisionSum;
            }

            readyBarcode = string.Concat(vector);

            //verificação de duplicidade de códigos de barra
            if (products != null)
            {
                invalidCode = false;
                foreach (var item in products) //verifica todos os códigos dos produtos, encontrando um igual ele repete o processo do while
                {
                    if (item.Barcode == readyBarcode)
                    {
                        invalidCode = true;
                        break;
                    }

                }

            }
            else //se não tiver produtos na lista, este vai ser o primeiro, não tendo com quem comparar
            {
                invalidCode = false;
            }

        }

        return readyBarcode;


    }
    public string SetName()
    {
        do
        {
            string name;
            bool IsValid = true;
            Console.Write("Digite o nome do produto: ");
            name = Console.ReadLine()!;

            if (!string.IsNullOrEmpty(name) && name.Length <= 20)
            {
                for (int i = 0; i < name.Length; i++)
                {
                    if (!char.IsLetterOrDigit(name, i))
                    {
                        IsValid = false;
                        break;
                    }
                }

                if (IsValid)
                {
                    return name;
                }

            }

        } while (true);
    }
    public decimal SetSalePrice()
    {
        Console.Write("Digite o valor de venda do produto: ");
        do
        {
            if (decimal.TryParse(Console.ReadLine(), out decimal price) && price > 0m && price <= 999.99m)
            {
                price = Math.Round(price, 2);
                if (price == 1000.00m)
                {
                    Console.Write("Digite um valor válido de venda do produto: ");
                    continue;
                }
                return price;
            }
            else
            {
                Console.Write("Digite um valor válido de venda do produto: ");
            }

        } while (true);


    }
    public DateTime SetCurrentDate()
    {
        return DateTime.Today;
    }
    public void SetInactive()
    {
        Status = EStatus.Inativo;
    }
    public override string ToString()
    {
        return $"""
            ----- DADOS DO PRODUTO -----
            Cód. de Barras: {Barcode}
            Nome: {Name} 
            Valor de Venda: {SalePrice.ToString("C")}
            Data do Cadastro: {RegistrationDate.ToString("dd/MM/yyyy")}
            Data Última Venda: {LastSaleDate.ToString("dd/MM/yyyy")}
            Status: {Status}
            """;
    }

    public string ToFileString()
    {
        return Barcode +
            Name.PadRight(20) +
            ((int)(SalePrice * 100)).ToString().PadLeft(5, '0') +
            RegistrationDate.ToString("ddMMyyyy") +
            LastSaleDate.ToString("ddMMyyyy") +
            Status.ToString()[0];
    }

    internal static bool ValidateFilePath(string filePath, bool createIfMissing = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("O caminho do arquivo não pode ser vazio.");
            return false;
        }
        else if (!File.Exists(filePath))
        {
            if (createIfMissing)
            {
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Create(filePath).Close();
                return true;
            }
            Console.WriteLine("O arquivo especificado não existe.");
            return false;
        }
        return true;
    }

    public static List<Product> ReadProdcutsFile(string pathFile)
    {
        List<Product> productsFileList = new List<Product>();


        if (ValidateFilePath(pathFile))
        {
            var reader = new StreamReader(pathFile);
            string? line = reader.ReadLine();

            while (line != null)
            {
                string barcode, name;
                decimal salePrice;
                DateTime registrationDate, lastSaleDate;
                EStatus status;

                barcode = line.Substring(0, 13);
                name = line.Substring(13, 20).Trim();
                salePrice = (decimal.Parse(line.Substring(33, 5))) / 100m;
                registrationDate = DateTime.ParseExact(line.Substring(38, 8), "ddMMyyyy", CultureInfo.InvariantCulture);
                lastSaleDate = DateTime.ParseExact(line.Substring(46, 8), "ddMMyyyy", CultureInfo.InvariantCulture);
                status = line.Substring(54, 1) == "A" ? EStatus.Ativo : EStatus.Inativo;

                Product p = new(barcode, name, salePrice, registrationDate, lastSaleDate, status);

                productsFileList.Add(p);
                line = reader.ReadLine();

            }
            reader.Close();
        }


        return productsFileList;
    }

    public static void SaveProductsFile(List<Product> productsList, string pathFile)
    {
        if (ValidateFilePath(pathFile, true))
        {

            var writer = new StreamWriter(pathFile);
            foreach (var item in productsList)
            {
                writer.WriteLine(item.ToFileString());
            }
            writer.Close();

        }
    }

}