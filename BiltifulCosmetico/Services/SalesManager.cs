
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BiltifulCosmetico.Classes;
using BiltifulCosmetico.Services;

public class SalesManager
{
    private readonly List<Sale> _sales = new();
    private readonly List<Product> _products;

    private int _nextSaleId = 1;
    private int _currentIndex = -1;

    private readonly Func<string, string?> _findCustomerByCPF;
    private readonly Func<string, DateOnly, bool> _updateLastPurchase;
    private readonly RiskFileRepository _riskRepository;

    // Construtor padrão: mantém compatibilidade com o código anterior.
    public SalesManager()
        : this(new List<Product>())
    {
    }

    // Construtor normal: recebe a lista de produtos carregada pelo sistema.
    public SalesManager(List<Product> products)
    {
        _products = products
            ?? throw new ArgumentNullException(nameof(products));

        _findCustomerByCPF = CustomerFile.FindCustomerByCPF;
        _updateLastPurchase = CustomerFile.UpdateLastPurchase;
        _riskRepository = new RiskFileRepository();
    }

    // Construtor para testes de clientes e risco, sem produtos.
    public SalesManager(
        Func<string, string?> findCustomerByCPF,
        Func<string, DateOnly, bool> updateLastPurchase,
        string riskFilePath)
        : this(
            new List<Product>(),
            findCustomerByCPF,
            updateLastPurchase,
            riskFilePath)
    {
    }

    // Construtor para testes com lista de produtos.
    public SalesManager(
        List<Product> products,
        Func<string, string?> findCustomerByCPF,
        Func<string, DateOnly, bool> updateLastPurchase,
        string riskFilePath)
    {
        _products = products
            ?? throw new ArgumentNullException(nameof(products));

        _findCustomerByCPF = findCustomerByCPF
            ?? throw new ArgumentNullException(nameof(findCustomerByCPF));

        _updateLastPurchase = updateLastPurchase
            ?? throw new ArgumentNullException(nameof(updateLastPurchase));

        if (string.IsNullOrWhiteSpace(riskFilePath))
        {
            throw new ArgumentException(
                "Caminho do arquivo de risco não informado.",
                nameof(riskFilePath)
            );
        }

        _riskRepository = new RiskFileRepository(riskFilePath);
    }

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

        if (sale.Items == null || sale.Items.Count < 1 || sale.Items.Count > 3)
        {
            throw new ArgumentException(
                "A venda deve possuir no mínimo 1 e no máximo 3 itens."
            );
        }

        // Valida os produtos e recupera os preços do cadastro.
        foreach (SaleItem item in sale.Items)
        {
            if (item == null)
            {
                throw new ArgumentException(
                    "A venda possui um item inválido."
                );
            }

            item.SaleId = sale.SaleId;

            Product? product = _products.FirstOrDefault(
                p => p.Barcode == item.ProductCode
            );

            if (product == null)
            {
                throw new InvalidOperationException(
                    $"Venda bloqueada: produto {item.ProductCode} não encontrado."
                );
            }

            if (product.Status != EStatus.Ativo)
            {
                throw new InvalidOperationException(
                    $"Venda bloqueada: produto {product.Name} está inativo."
                );
            }

            // O preço da venda sempre vem do cadastro do produto.
            item.UnitPrice = product.SalePrice;
        }

        // Valida os dados da venda e seus itens.
        sale.Validate();

        // Verifica se o cliente está cadastrado.
        string? registro = _findCustomerByCPF(sale.ClientCPF);

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
        if (_riskRepository.IsCpfAtRisk(customer.CPF))
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
        if (!_updateLastPurchase(customer.CPF, sale.SaleDate))
        {
            throw new InvalidOperationException(
                "Não foi possível atualizar a data da última compra do cliente."
            );
        }

        // Registra a venda na lista em memória.
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
            throw new ArgumentNullException(nameof(fileManager));
        }

        bool deleted = fileManager.DeleteSale(saleId);

        if (!deleted)
            return false;

        Sale? sale = FindSale(saleId);

        if (sale != null)
            _sales.Remove(sale);

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
            sale.Validate();

        _sales.Clear();
        _sales.AddRange(loadedSales);

        _sales.Sort(
            (a, b) => a.SaleId.CompareTo(b.SaleId)
        );

        _nextSaleId = _sales.Count == 0
            ? 1
            : _sales.Max(sale => sale.SaleId) + 1;

        _currentIndex = -1;
    }
}
