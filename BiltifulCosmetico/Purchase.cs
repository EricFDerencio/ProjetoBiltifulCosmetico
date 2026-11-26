public class Purchase
{
    public int Id { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public string SupplierCnpj {  get; private set; }
    public decimal TotalValue { get; private set; }

    //construtor
    public Purchase(int id, string supplierCnpj)
    {
        Id = id;
        PurchaseDate = DateTime.Today; //parametro para pegar a data de hora atual da maquina
        SupplierCnpj = supplierCnpj;
        TotalValue = 0;
    }

    //metodo para somar itens
    public void AddItemValue(decimal itemTotal)
    {
        TotalValue += itemTotal;
    }

    public string ToFileLine()
    {
        string id = Id.ToString().PadLeft(5, '0'); //PadLeft serve para preencher o restante dos valores esperados com o caracter '0'
        string date = PurchaseDate.ToString("ddMMyyyy");
        string cnpj = SupplierCnpj.PadLeft(14, '0');

        long cents = (long)(TotalValue * 100);
        string total = cents.ToString().PadLeft(7, '0');

        return id + date + cnpj + total;
    }
}
