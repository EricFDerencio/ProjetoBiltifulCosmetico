public class Sale
{
    public int SaleId {  get; set; }
    public DateOnly SaleDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string ClientCPF { get; set; } = string.Empty;
    public List<SaleItem> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(item => item.ItemTotal);

    public void Validate()
    {
        //validação do ID
        if (SaleId < 1 || SaleId > 99999)
        {
            throw new ArgumentOutOfRangeException(nameof(SaleId), "O código da venda deve estar entre 1 e 99999.");
        }

        //validação do cpf
        if (!CpfValidator.IsValid(ClientCPF))
        {
            throw new ArgumentException("O CPF do cliente é inválido.");
        }

        //Normaliza o cpf
        ClientCPF = CpfValidator.Normalize(ClientCPF);

        //validação da quantidade de itens
        if (Items == null || Items.Count < 1 || Items.Count > 3)
        {
            throw new ArgumentException("A venda deve possuir no mínimo 1 e no máxim 3 itens.");
        }

        //validação de cada item
        foreach (SaleItem item in Items)
        {
            if (item == null)
            {
                throw new ArgumentException("A venda possui um item inválido.");
            }

            if (item.SaleId != SaleId)
            {
                throw new ArgumentException("O ID do item não corresponde ao ID da venda.");
            }

            item.Validate();
        }

        // validação do total da venda
        if (TotalAmount <= 0 || TotalAmount > 99999.99m)
        {
            throw new ArgumentOutOfRangeException(nameof(TotalAmount), "O total da venda deve ser maior que zero e não ultrapassar R$ 99.999,99.");
        }

    }

}