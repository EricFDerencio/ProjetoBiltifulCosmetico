public class SaleItem
{
    public int SaleId {  get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ItemTotal => Quantity * UnitPrice;


}