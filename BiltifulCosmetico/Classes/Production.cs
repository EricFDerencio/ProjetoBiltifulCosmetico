
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class Production
{
    public int Id { get; set; }

    private static int nextId = 0;

    public DateTime ProductionDate { get; set; }

    public string Product { get; set; }

    public float Quantity { get; set; }

    public static List<Production> list = SetList();

    private static string FilePath =>
        Path.Combine(Utils.path, Utils.file);

    public Production(string p, float q)
    {
        ValidateProductCode(p);
        ValidateQuantity(q);

        Id = setId();
        ProductionDate = DateTime.Today;
        Product = p;
        Quantity = q;
    }

    public Production(int id, DateTime date, string prod, float quant)
    {
        if (id < 1 || id > 99999)
            throw new ArgumentOutOfRangeException(
                nameof(id),
                "O ID deve estar entre 1 e 99999."
            );

        ValidateProductCode(prod);
        ValidateQuantity(quant);

        Id = id;
        ProductionDate = date;
        Product = prod;
        Quantity = quant;
    }

    private static void ValidateProductCode(string productCode)
    {
        if (string.IsNullOrWhiteSpace(productCode) ||
            productCode.Length != 13 ||
            !productCode.StartsWith("789") ||
            !productCode.All(char.IsAsciiDigit))
        {
            throw new ArgumentException(
                "O código de barras deve possuir 13 dígitos e começar com 789.",
                nameof(productCode)
            );
        }
    }

    private static void ValidateQuantity(float quantity)
    {
        if (float.IsNaN(quantity) ||
            float.IsInfinity(quantity) ||
            quantity <= 0 ||
            quantity > 999.99f)
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantity),
                "A quantidade deve ser maior que zero e não exceder 999.99."
            );
        }
    }

    public static List<Production> SetList()
    {
        List<Production> productions = new();

        string path = EnsureFileExists();

        foreach (string line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('|');

            if (parts.Length != 4)
                throw new InvalidDataException(
                    "Registro inválido no arquivo de produção."
                );

            try
            {
                int id = int.Parse(
                    parts[0],
                    CultureInfo.InvariantCulture
                );

                DateTime date = DateTime.ParseExact(
                    parts[1],
                    "ddMMyyyy",
                    CultureInfo.InvariantCulture
                );

                string productCode = parts[2];

                float quantity = float.Parse(
                    parts[3],
                    CultureInfo.InvariantCulture
                );

                productions.Add(
                    new Production(id, date, productCode, quantity)
                );
            }
            catch (Exception ex) when (
                ex is FormatException ||
                ex is OverflowException ||
                ex is ArgumentException
            )
            {
                throw new InvalidDataException(
                    "Não foi possível interpretar um registro de produção.",
                    ex
                );
            }
        }

        if (productions.GroupBy(p => p.Id).Any(g => g.Count() > 1))
            throw new InvalidDataException(
                "Existem IDs de produção duplicados."
            );

        nextId = productions.Count == 0
            ? 0
            : productions.Max(p => p.Id);

        return productions;
    }

    private static string EnsureFileExists()
    {
        string directory = Path.GetDirectoryName(FilePath)
            ?? throw new InvalidOperationException(
                "Diretório de produção inválido."
            );

        Directory.CreateDirectory(directory);

        if (!File.Exists(FilePath))
        {
            using FileStream file = File.Create(FilePath);
        }

        return FilePath;
    }

    public int setId()
    {
        if (nextId >= 99999)
            throw new InvalidOperationException(
                "Limite de IDs de produção atingido."
            );

        nextId++;
        Id = nextId;

        return Id;
    }

    public static bool PathValidation(string p)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(p))
                return false;

            Directory.CreateDirectory(p);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public static bool FileValidation(string p)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(p))
                return false;

            string? directory = Path.GetDirectoryName(p);

            if (!string.IsNullOrWhiteSpace(directory))
                Directory.CreateDirectory(directory);

            if (!File.Exists(p))
            {
                using FileStream file = File.Create(p);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public void Register(Production p)
    {
        ArgumentNullException.ThrowIfNull(p);

        EnsureFileExists();

        if (list.Any(item => item.Id == p.Id))
            throw new InvalidOperationException(
                "Já existe uma produção com esse ID."
            );

        File.AppendAllText(
            FilePath,
            ToFileString(p) + Environment.NewLine
        );

        list.Add(p);

        if (p.Id > nextId)
            nextId = p.Id;
    }

    private static string ToFileString(Production p)
    {
        return p.Id.ToString(CultureInfo.InvariantCulture) + "|" +
               p.ProductionDate.ToString(
                   "ddMMyyyy",
                   CultureInfo.InvariantCulture
               ) + "|" +
               p.Product + "|" +
               p.Quantity.ToString(
                   "0.00",
                   CultureInfo.InvariantCulture
               );
    }

    public string Localizar(int id)
    {
        Production? encontrada = list.Find(p => p.Id == id);

        if (encontrada == null)
            return "Produção não encontrada";

        return encontrada.ToString();
    }

    public bool Delete(int id)
    {
        Production? production = list.Find(p => p.Id == id);

        if (production == null)
            return false;

        list.Remove(production);

        List<string> records = list
            .Select(ToFileString)
            .ToList();

        File.WriteAllLines(FilePath, records);

        return true;
    }

    public List<Production> Listar()
    {
        return list;
    }

    public override string ToString()
    {
        return $"""
            ID: {Id}
            Data de produção: {ProductionDate:dd/MM/yyyy}
            Código de barras: {Product}
            Quantidade: {Quantity}
            """;
    }
}
