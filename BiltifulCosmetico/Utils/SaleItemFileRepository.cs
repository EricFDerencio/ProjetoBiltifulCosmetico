using System.Globalization;

public class SaleItemFileRepository
{
    private readonly string _filePath;

    public SaleItemFileRepository(string filePath = null)
    {
        string directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "biltiful"
        );

        Directory.CreateDirectory(directory);

        _filePath = filePath ?? Path.Combine(directory, "ItemVenda.dat");
    }

    public void SaveAll(IEnumerable<SaleItem> items)
    {
        if (items == null)
        {
            throw new ArgumentNullException(
                nameof(items),
                "A lista de itens não pode ser nula."
            );
        }

        List<SaleItem> itemList = items.ToList();

        if (itemList.Any(item => item == null))
        {
            throw new InvalidDataException(
                "A lista contém um item inválido."
            );
        }

        if (itemList.GroupBy(item => item.SaleId)
                    .Any(group => group.Count() > 3))
        {
            throw new InvalidDataException(
                "Uma venda não pode possuir mais de 3 itens."
            );
        }

        List<string> lines = new();

        foreach (SaleItem item in itemList)
        {
            item.Validate();

            string saleId = item.SaleId.ToString("D5");

            string productCode = item.ProductCode;

            string quantity = item.Quantity.ToString("D3");

            long unitPriceInCents = decimal.ToInt64(
                item.UnitPrice * 100m
            );

            string unitPrice = unitPriceInCents.ToString("D5");

            long itemTotalInCents = decimal.ToInt64(
                item.ItemTotal * 100m
            );

            string itemTotal = itemTotalInCents.ToString("D6");

            string line =
                saleId +
                productCode +
                quantity +
                unitPrice +
                itemTotal;

            if (line.Length != 32)
            {
                throw new InvalidDataException(
                    "O registro do item possui tamanho incorreto."
                );
            }

            lines.Add(line);
        }

        File.WriteAllLines(_filePath, lines);
    }

    public List<SaleItem> LoadAll()
    {
        List<SaleItem> items = new();

        if (!File.Exists(_filePath))
            return items;

        int lineNumber = 0;

        foreach (string line in File.ReadLines(_filePath))
        {
            lineNumber++;

            if (line.Length != 32)
            {
                throw new InvalidDataException(
                    $"Registro inválido na linha {lineNumber}: tamanho incorreto."
                );
            }

            try
            {
                string saleIdText = line.Substring(0, 5);
                string productCode = line.Substring(5, 13);
                string quantityText = line.Substring(18, 3);
                string unitPriceText = line.Substring(21, 5);
                string itemTotalText = line.Substring(26, 6);

                int saleId = int.Parse(
                    saleIdText,
                    CultureInfo.InvariantCulture
                );

                int quantity = int.Parse(
                    quantityText,
                    CultureInfo.InvariantCulture
                );

                long unitPriceInCents = long.Parse(
                    unitPriceText,
                    CultureInfo.InvariantCulture
                );

                long itemTotalInCents = long.Parse(
                    itemTotalText,
                    CultureInfo.InvariantCulture
                );

                decimal unitPrice = unitPriceInCents / 100m;

                decimal storedItemTotal = itemTotalInCents / 100m;

                SaleItem item = new()
                {
                    SaleId = saleId,
                    ProductCode = productCode,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                };

                item.Validate();

                if (item.ItemTotal != storedItemTotal)
                {
                    throw new InvalidDataException(
                        "O total gravado não corresponde à quantidade multiplicada pelo preço unitário."
                    );
                }

                items.Add(item);
            }
            catch (Exception ex) when (
                ex is FormatException ||
                ex is OverflowException ||
                ex is ArgumentException
            )
            {
                throw new InvalidDataException(
                    $"Não foi possível interpretar a linha {lineNumber} do arquivo ItemVenda.dat.",
                    ex
                );
            }
        }

        if (items.GroupBy(item => item.SaleId)
                 .Any(group => group.Count() > 3))
        {
            throw new InvalidDataException(
                "O arquivo possui uma venda com mais de 3 itens."
            );
        }

        return items;
    }
}