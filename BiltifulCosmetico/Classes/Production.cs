using BiltifulCosmetico.Dados;
using Microsoft.Win32;
﻿
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
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
        public int Id = 0;
        static int nextId = 0;
        public DateTime ProductionDate { get; set; }
        public string Product { get; set; }
        public float Quantity { get; set; }
        public static List<Production> list = SetList();

        public Production(string p, float q)
        {
            this.Id = setId();
            this.ProductionDate = DateTime.Today;
            this.Product = p;
            if (q > 999.99)
            {
                throw new ArgumentOutOfRangeException(nameof(q), "A quantidade não deve exceder de 999.99");
            }
            else
            {
                this.Quantity = q;
            }
        }

        public Production(int id, DateTime date, string prod, float quant)
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
            List<Production> list = new List<Production>();
            var reader = new StreamReader(Dados.ProductionFiles.caminho);
            string aux = string.Empty;
            if (Dados.ProductionFiles.FileValidation(Dados.ProductionFiles.caminho))
            {
                while (aux is not null)
                {
                    aux = reader.ReadLine();
                    if (!string.IsNullOrEmpty(aux))
                    {
                        string idreader = aux.Substring(0, 4);
                        int id = int.Parse(idreader.Trim());

                        string datereader = aux.Substring(5, 10);
                        DateTime date = DateTime.ParseExact(datereader.Trim(), "ddMMyyyy", CultureInfo.InvariantCulture);

                        string prodreader = aux.Substring(15, 13);
                        string prod = prodreader.Trim();

                        string quantityreader = aux.Substring(31, 5);
                        float quan = float.Parse(quantityreader.Trim());

                        Production p = new(id, date, prod, quan);
                        list.Add(p);
                    }
                }
                reader.Close();
            }
            return list;
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
                using (var reader = new StreamReader(Dados.ProductionFiles.caminho))
                {
                    string? linha;

                    while ((linha = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(linha))
                        {
                            string idreader = linha.Substring(0, 5);

                            nextId = int.Parse(idreader.Trim());
                        }
                    }
                }
            }

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
            if (Dados.ProductionFiles.FileValidation(Dados.ProductionFiles.caminho))
            {
                list.Add(p);
                string register = Id.ToString().PadRight(5, ' ') +
                    ProductionDate.ToString("ddMMyyyy").PadRight(10, ' ') +
                    Product.PadRight(20, ' ') +
                    Quantity.ToString().PadRight(10, ' ');

                File.AppendAllText(Dados.ProductionFiles.caminho, "\n" + register);
            }
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
            Production encontrada = list.Find(p => p.Id == id);
            if (encontrada == null)
            {
                return "Produção não encontrada";
            }
            return encontrada.ToString();
        }

        public bool Delete(int id)
        {
            Production enc = list.Find(p => p.Id == id);
            if (enc == null)
            {
                return false;
            }
            list.Remove(enc);
            string register;
            List<string> dados = new List<string>();
            foreach (var item in list)
            {
                register = item.Id.ToString().PadRight(5, ' ') +
                    item.ProductionDate.ToString("ddMMyyyy").PadRight(10, ' ') +
                    item.Product.PadRight(20, ' ') +
                    item.Quantity.ToString().PadRight(10, ' ');
                dados.Add(register);
            }

            return true;
        }
        catch (Exception ex)
        {
            return list;
        }
    }

        public override string ToString()
        {
            return $""" Id - {this.Id:D5} - Data de produção - {this.ProductionDate}: - Produto - {this.Product} - Quantidade - {this.Quantity} """;
        }
    }
}