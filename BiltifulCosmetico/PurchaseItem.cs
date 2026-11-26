public class PurchaseItem
{
    public int Id { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public string RawMaterialId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitValue { get; private set; }
    public decimal ItemTotal { get; private set; }

    public PurchaseItem(int id, DateTime purchaseDate, string rawMaterialId, decimal quantity, decimal unitValue)
    {
        Id = id;
        PurchaseDate = purchaseDate;
        RawMaterialId = rawMaterialId;
        Quantity = quantity;
        UnitValue = unitValue;
        ItemTotal = quantity * unitValue;
    }

    public string ToFileLine()
    {
        string id = Id.ToString().PadLeft(5, '0');
        string purchaseItem = PurchaseDate.ToString("ddMMyyyy");
        string rawMaterialId = RawMaterialId.ToString().PadLeft(6, '0');

        //valores com casa decimal, retirando a vígula para transformar em string sem separacao
        long totalq = (long)(Quantity * 100);
        string quantity = totalq.ToString().PadLeft(5, '0');
        long totalu = (long)(UnitValue * 100);
        string unitValue = totalu.ToString().PadLeft(5, '0');
        long totali = (long)(ItemTotal * 100);
        string itemTotal = totali.ToString().PadLeft(5, '0');

        return id + purchaseItem + rawMaterialId + quantity + unitValue + itemTotal;
    }
}
