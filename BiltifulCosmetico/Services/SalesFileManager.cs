public class SalesFileManager
{
    private readonly SaleFileRepository _saleRepository;
    private readonly SaleItemFileRepository _itemRepository;

    public SalesFileManager(
        SaleFileRepository saleRepository,
        SaleItemFileRepository itemRepository)
    {
        _saleRepository = saleRepository ??
            throw new ArgumentNullException(nameof(saleRepository));

        _itemRepository = itemRepository ??
            throw new ArgumentNullException(nameof(itemRepository));
    }

    public List<Sale> LoadAll()
    {
        List<Sale> sales = _saleRepository.LoadAll();
        List<SaleItem> items = _itemRepository.LoadAll();

        if (sales.GroupBy(sale => sale.SaleId)
                 .Any(group => group.Count() > 1))
        {
            throw new InvalidDataException(
                "O arquivo Venda.dat possui códigos de venda duplicados."
            );
        }

        foreach (SaleItem item in items)
        {
            if (!sales.Any(sale => sale.SaleId == item.SaleId))
            {
                throw new InvalidDataException(
                    $"O item está associado à venda {item.SaleId}, que não existe."
                );
            }
        }

        foreach (Sale sale in sales)
        {
            sale.Items = items
                .Where(item => item.SaleId == sale.SaleId)
                .ToList();

            sale.Validate();

            if (sale.StoredTotalAmount != sale.TotalAmount)
            {
                throw new InvalidDataException(
                    $"O total da venda {sale.SaleId} não corresponde à soma dos itens."
                );
            }
        }

        return sales;
    }

    public void SaveAll(IEnumerable<Sale> sales)
    {
        if (sales == null)
        {
            throw new ArgumentNullException(
                nameof(sales),
                "A lista de vendas não pode ser nula."
            );
        }

        List<Sale> saleList = sales.ToList();

        if (saleList.Any(sale => sale == null))
        {
            throw new InvalidDataException(
                "A lista contém uma venda inválida."
            );
        }

        if (saleList.GroupBy(sale => sale.SaleId)
                    .Any(group => group.Count() > 1))
        {
            throw new InvalidDataException(
                "Existem vendas com códigos duplicados."
            );
        }

        // Valida todas as vendas antes de gravar os arquivos.
        foreach (Sale sale in saleList)
        {
            sale.Validate();
        }

        // Reúne todos os itens de todas as vendas.
        List<SaleItem> items = saleList
            .SelectMany(sale => sale.Items)
            .ToList();

        // Grava os itens e depois os dados principais das vendas.
        _itemRepository.SaveAll(items);
        _saleRepository.SaveAll(saleList);
    }

    public bool DeleteSale(int saleId)
    {
        // Carrega as vendas com seus respectivos itens.
        List<Sale> sales = LoadAll();

        // Localiza a venda pelo código.
        Sale? sale = sales.FirstOrDefault(
            s => s.SaleId == saleId
        );

        // Retorna false caso a venda não exista.
        if (sale == null)
            return false;

        // Remove a venda da lista.
        sales.Remove(sale);

        // Atualiza os arquivos de vendas e itens.
        SaveAll(sales);

        return true;
    }
}