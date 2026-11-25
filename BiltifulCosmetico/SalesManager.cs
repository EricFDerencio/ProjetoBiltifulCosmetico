public class SalesManager
{
    private readonly List<Sale> _sales = new();
    private int _nextSaleId = 1;

    //posição da venda atualmente selecionada na navegação.
    private int _currentIndex = -1;

    public Sale RegisterSale(Sale sale)
    {
        if (sale == null)
        {
            throw new ArgumentNullException(
                nameof(sale),
                "A venda não pode ser nula."
            );
        }

        if (_nextSaleId > 99999)
        {
            throw new InvalidOperationException(
                "Limite máximo de vendas atingido."
            );
        }

        sale.SaleId = _nextSaleId;
        sale.SaleDate = DateOnly.FromDateTime(DateTime.Today);

        if (sale.Items != null)
        {
            foreach (SaleItem item in sale.Items)
            {
                if (item != null)
                    item.SaleId = sale.SaleId;
            }
        }

        sale.Validate();

        if (_sales.Any(s => s.SaleId == sale.SaleId))
        {
            throw new InvalidOperationException(
                "Já existe uma venda com esse código."
            );
        }

        _sales.Add(sale);
        _nextSaleId++;

        return sale;
    }

    public Sale? FindSale(int saleId)
    {
        return _sales.FirstOrDefault(
            s => s.SaleId == saleId
        );
    }

    public bool DeleteSale(int saleId)
    {
        Sale? sale = FindSale(saleId);

        if (sale == null)
            return false;

        _sales.Remove(sale);

        //reinicia a navegação para evitar uma posição desatualizada.
        _currentIndex = -1;

        return true;
    }

    public IReadOnlyList<Sale> GetAllSales()
    {
        return _sales.AsReadOnly();
    }

    public Sale? GetFirstSale()
    {
        if (_sales.Count == 0)
            return null;

        _currentIndex = 0;

        return _sales[_currentIndex];
    }

    public Sale? GetLastSale()
    {
        if (_sales.Count == 0)
            return null;

        _currentIndex = _sales.Count - 1;

        return _sales[_currentIndex];
    }

    public Sale? GetNextSale()
    {
        if (_sales.Count == 0)
            return null;

        if (_currentIndex == -1)
            return GetFirstSale();

        if (_currentIndex >= _sales.Count - 1)
            return null;

        _currentIndex++;

        return _sales[_currentIndex];
    }

    public Sale? GetPreviousSale()
    {
        if (_sales.Count == 0)
            return null;

        if (_currentIndex == -1)
            return GetLastSale();

        if (_currentIndex <= 0)
            return null;

        _currentIndex--;

        return _sales[_currentIndex];
    }
}