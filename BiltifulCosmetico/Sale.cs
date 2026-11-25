public class Sale
{
    public int Saled {  get; set; }
    public DateOnly SaleDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string ClientCPF { get; set; }
    public decimal TotalAmount { get; set; }



}