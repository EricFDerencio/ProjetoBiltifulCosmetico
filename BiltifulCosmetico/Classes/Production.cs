using System;
using System.Collections.Generic;
using System.Text;

namespace BiltifulCosmetico.Classes
{
    internal class Production
    {
        public int Id = 0;
        static int nextId = 0;
        public DateTime ProductionDate { get; set; }
        public int Product { get; set; }
        public float Quantity { get; set; }
        public List<Production> lista { get; set; }
        
        public Production(int p, float q)
        {
            nextId++;
            if (nextId > 99999)
            {
                throw new InvalidOperationException("Limite de IDs atingido");
            }
            else
            {
                this.Id = nextId;
            }
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

        public string Cadastrar(Production p)
        {
            this.lista.Append(p);
            return $"""ID: {p.Id}, Data de produção: {p.ProductionDate.ToString("ddMMyyyy")}, Id do produto: {p.Product}, Quantidade: {p.Quantity}""";
        }

        public Production Localizar(int id)
        {
            Production encontrada = lista.Find(p => p.Id == id);
            return encontrada;
        }
        
        public Production Excluir(int id)
        {
            Production enc = lista.Find(p => p.Id == id);
            lista.Remove(enc);
            return enc;
        }

        public List<Production> Listar()
        {
            return lista;
        }
    }
}
