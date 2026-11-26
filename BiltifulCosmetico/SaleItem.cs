public class SaleItem
{
    public int SaleId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ItemTotal => Quantity * UnitPrice;

    public void Validate()
    {
        //validação do código da venda
        if (SaleId < 1 || SaleId > 99999)
        {
            throw new ArgumentOutOfRangeException(nameof(SaleId), "O código da venda deve estar entre 1 e 99999.");
        }

        // validação do código do produto
        if (string.IsNullOrWhiteSpace(ProductCode) || ProductCode.Length != 13 || ProductCode.StartsWith("789") || !IsNumeric(ProductCode))
        {
            throw new ArgumentException("O código do produto deve possuir 13 digitos e iniciar com 789.");
        }

        //validação da quantidade
        if (Quantity < 1 || Quantity > 999)
        {
            throw new ArgumentOutOfRangeException(nameof(Quantity), "A quantidade deve estar entre 1 e 999.");
        }

        //validação do preço unitário
        if (UnitPrice <= 0 || UnitPrice > 999.99m)
        {
            throw new ArgumentOutOfRangeException(nameof(UnitPrice), "O preço unitário deve ser maior que zero e não ulttrapassar R$ 999,99.");
        }

        //validação das casas decimais
        if (decimal.Round(UnitPrice, 2) !=UnitPrice)
        {
            throw new ArgumentException("O preço unitário deve possuir no máximo duas casas decimais.");
        }

        //validação do total do item
        if (ItemTotal > 999.99m)
        {
            throw new ArgumentOutOfRangeException(nameof(ItemTotal), "O total do item não pode ultrapassar R$ 9.999,99.");
        }
    }

    private static bool IsNumeric(string value)
    {
        foreach (char character in value)
        {
            if(!char.IsAsciiDigit(character))
                return false;
        }
        return true;
    }

}