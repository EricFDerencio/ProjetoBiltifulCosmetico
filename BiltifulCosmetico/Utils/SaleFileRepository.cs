using System.Globalization;

public class SaleFileRepository
{
    private readonly string _filePath;

    public SaleFileRepository(string filePath = "Venda.dat")
    {
        _filePath = filePath;
    }

    public void SaveAll(IEnumerable<Sale> sales)
    {
        if (sales == null)
        {
            throw new ArgumentNullException(
                nameof(sales),
                "A lista de vendas não pode ser nula."
            );
        }

        List<string> lines = new();

        foreach (Sale sale in sales)
        {
            if (sale == null)
            {
                throw new InvalidDataException(
                    "A lista contém uma venda inválida."
                );
            }

            sale.Validate();

            string id = sale.SaleId.ToString("D5");

            string date = sale.SaleDate.ToString(
                "ddMMyyyy",
                CultureInfo.InvariantCulture
            );

            string cpf = CpfValidator.Normalize(sale.ClientCPF);

            long totalInCents = decimal.ToInt64(
                sale.TotalAmount * 100m
            );

            string total = totalInCents.ToString("D7");

            string line = id + date + cpf + total;

            if (line.Length != 31)
            {
                throw new InvalidDataException(
                    "O registro da venda possui tamanho incorreto."
                );
            }

            lines.Add(line);
        }

        File.WriteAllLines(_filePath, lines);
    }

    public List<Sale> LoadAll()
    {
        List<Sale> sales = new();

        if (!File.Exists(_filePath))
            return sales;

        int lineNumber = 0;

        foreach (string line in File.ReadLines(_filePath))
        {
            lineNumber++;

            if (line.Length != 31)
            {
                throw new InvalidDataException(
                    $"Registro inválido na linha {lineNumber}: tamanho incorreto."
                );
            }

            try
            {
                string idText = line.Substring(0, 5);
                string dateText = line.Substring(5, 8);
                string cpf = line.Substring(13, 11);
                string totalText = line.Substring(24, 7);

                int saleId = int.Parse(
                    idText,
                    CultureInfo.InvariantCulture
                );

                DateOnly saleDate = DateOnly.ParseExact(
                    dateText,
                    "ddMMyyyy",
                    CultureInfo.InvariantCulture
                );

                long totalInCents = long.Parse(
                    totalText,
                    CultureInfo.InvariantCulture
                );

                decimal total = totalInCents / 100m;

                if (!CpfValidator.IsValid(cpf))
                {
                    throw new InvalidDataException(
                        "CPF inválido no registro."
                    );
                }

                if (saleId < 1 || saleId > 99999)
                {
                    throw new InvalidDataException(
                        "Código de venda inválido."
                    );
                }

                if (total <= 0 || total > 99999.99m)
                {
                    throw new InvalidDataException(
                        "Valor total da venda inválido."
                    );
                }

                Sale sale = new()
                {
                    SaleId = saleId,
                    SaleDate = saleDate,
                    ClientCPF = cpf,
                    StoredTotalAmount = total
                };

                sales.Add(sale);
            }
            catch (Exception ex) when (
                ex is FormatException ||
                ex is OverflowException ||
                ex is ArgumentException
            )
            {
                throw new InvalidDataException(
                    $"Não foi possível interpretar a linha {lineNumber} do arquivo Venda.dat.",
                    ex
                );
            }
        }

        return sales;
    }
}