using BiltifulCosmetico.Dados;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;

namespace BiltifulCosmetico.Classes
{
    internal class Production
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
            this.Id = id;
            this.ProductionDate = date;
            this.Product = prod;
            this.Quantity = quant;
        }

        public static List<Production> SetList()
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

        public int setId()
        {
            nextId = 0;

            if (Dados.ProductionFiles.FileValidation(Dados.ProductionFiles.caminho))
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

            nextId++;

            if (nextId > 99999)
            {
                throw new InvalidOperationException("Limite de IDs atingido");
            }

            Id = nextId;

            return Id;
        }

        public void Register(Production p)
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

        public string Localizar(int id)
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
            File.WriteAllLines(Dados.ProductionFiles.caminho, dados);
            return true;
        }

        public List<Production> Listar()
        {
            return list;
        }

        public override string ToString()
        {
            return $""" Id: {this.Id:D5} - Data de produção: {this.ProductionDate}: - Produto: {this.Product} - Quantidade: {this.Quantity} """;
        }
    }
}