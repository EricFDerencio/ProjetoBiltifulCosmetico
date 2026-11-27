
using BiltifulCosmetico.Classes;
using BiltifulCosmetico.Services;

public class SalesManager
{
    private readonly List<Sale> _sales = new();
    private int _nextSaleId = 1;
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

        // Valida os dados da venda e seus itens.
        sale.Validate();

        // Verifica se o cliente está cadastrado.
        string? registro = CustomerFile.FindCustomerByCPF(sale.ClientCPF);

        if (registro == null)
        {
            throw new InvalidOperationException(
                "Venda bloqueada: cliente não encontrado."
            );
        }

        // Recupera os dados do cliente.
        Customer customer = new Customer(registro);

        // Verifica se o cliente está ativo.
        if (customer.Situation != ESituation.Active)
        {
            throw new InvalidOperationException(
                "Venda bloqueada: cliente inativo."
            );
        }

        // Verifica se o cliente é maior de idade.
        if (!customer.IsAdult())
        {
            throw new InvalidOperationException(
                "Venda bloqueada: cliente menor de 18 anos."
            );
        }

        // Verifica se o CPF consta na lista de risco.
        RiskFileRepository riskRepository = new RiskFileRepository();

        if (riskRepository.IsCpfAtRisk(customer.CPF))
        {
            throw new InvalidOperationException(
                "Venda bloqueada: CPF consta na lista de risco."
            );
        }

        // Verifica se já existe uma venda com o mesmo código.
        if (_sales.Any(s => s.SaleId == sale.SaleId))
        {
            throw new InvalidOperationException(
                "Já existe uma venda com esse código."
            );
        }

        // Atualiza a data da última compra do cliente.
        if (!CustomerFile.UpdateLastPurchase(
            customer.CPF,
            sale.SaleDate))
        {
            throw new InvalidOperationException(
                "Não foi possível atualizar a data da última compra do cliente."
            );
        }

        // Registra a venda na lista.
        _sales.Add(sale);

        _nextSaleId++;

        return sale;
    }

    public Sale? FindSale(int saleId)
    {
        return _sales.FirstOrDefault(
            sale => sale.SaleId == saleId
        );
    }

    public bool DeleteSale(int saleId, SalesFileManager fileManager)
    {
        if (fileManager == null)
        {
            throw new ArgumentNullException(
                nameof(fileManager)
            );
        }

        // Exclui a venda e seus itens dos arquivos.
        bool deleted = fileManager.DeleteSale(saleId);

        if (!deleted)
            return false;

        // Remove a venda da lista em memória.
        Sale? sale = FindSale(saleId);

        if (sale != null)
        {
            _sales.Remove(sale);
        }

        // Reinicia a navegação.
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

    public void LoadSales(IEnumerable<Sale> sales)
    {
        if (sales == null)
        {
            throw new ArgumentNullException(
                nameof(sales),
                "A lista de vendas não pode ser nula."
            );
        }

        List<Sale> loadedSales = sales.ToList();

        if (loadedSales.Any(sale => sale == null))
        {
            throw new InvalidDataException(
                "A lista contém uma venda inválida."
            );
        }

        if (loadedSales.GroupBy(sale => sale.SaleId)
                       .Any(group => group.Count() > 1))
        {
            throw new InvalidDataException(
                "Existem vendas com códigos duplicados."
            );
        }

        foreach (Sale sale in loadedSales)
        {
            sale.Validate();
        }

        // Substitui as vendas que estão atualmente em memória.
        _sales.Clear();
        _sales.AddRange(loadedSales);

        _sales.Sort(
            (a, b) => a.SaleId.CompareTo(b.SaleId)
        );

        // Define o próximo código disponível.
        _nextSaleId = _sales.Count == 0
            ? 1
            : _sales.Max(sale => sale.SaleId) + 1;

        // Reinicia a navegação.
        _currentIndex = -1;
    }
}
