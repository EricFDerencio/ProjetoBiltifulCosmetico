public class Production
{
    internal class Production
    {
        public int Id = 0;
        static int nextId = 0;
        public DateTime ProductionDate { get; set; }
        public int Product { get; set; }
        public float Quantity { get; set; }
        public static List<Production> list = SetList();
        
        public Production(int p, float q)
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

        public Production(int id, DateTime date, int prod, float quant)
        {
            this.Id = id;
            this.ProductionDate = date;
            this.Product = prod;
            this.Quantity = quant;
        }

        public static List<Production> SetList()
        {
            List<Production> list = new List<Production>();
            var reader = new StreamReader(Utils.path + Utils.file);
            string aux = string.Empty;
            if (FileValidation(Utils.path + Utils.file))
            {
                while (aux is not null)
                {
                    aux = reader.ReadLine();
                    if (!string.IsNullOrEmpty(aux))
                    {
                        string[] partes = aux.Split("-");
                        int id = int.Parse(partes[1].Trim());
                        DateTime date = DateTime.Parse(partes[3].Trim().TrimEnd(":"), new CultureInfo("pt-br"));
                        int prod = int.Parse(partes[5].Trim());
                        float quan = float.Parse(partes[7].Trim());
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
            if (FileValidation(Utils.path + Utils.file))
            {
                var reader = new StreamReader(Utils.path + Utils.file);
                string leitor = string.Empty;
                string aux = string.Empty;
                do
                {
                    leitor = aux;
                    aux = reader.ReadLine();
                } while (aux is not null && aux != string.Empty);
                reader.Close();
                if(leitor != string.Empty)
                {
                    string? linha;

                    while ((linha = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(linha))
                        {
                            string[] partes = linha.Split("-");

                            if (partes.Length > 1 &&
                                int.TryParse(partes[1].Trim(), out int idLido))
                            {
                                nextId = idLido;
                            }
                        }
                    }
                }
            }

            nextId++;

            if (nextId > 99999)
            {
                throw new InvalidOperationException("Limite de IDs atingido");
            }
            else
            {
                this.Id = nextId;
            }
            return this.Id;
        }
        public static bool PathValidation(string p)
        {
            try
            {
                Directory.CreateDirectory(p);
                return true;
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public static bool FileValidation(string p)
        {
            if (File.Exists(p))
                return true;
            try
            {
                if (PathValidation(p))
                {
                    File.Create(p + Utils.file).Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return false;
        }

        public void Register(Production p)
        {
            if (FileValidation(Utils.path + Utils.file))
            {
                list.Add(p);
                File.AppendAllText(Utils.path + Utils.file, "\n" + p.ToString());
            }
        }

        public string Localizar(int id)
        {
            Production encontrada = list.Find(p => p.Id == id);
            if(encontrada == null)
            {
                return "Produção não encontrada";
            }
            return encontrada.ToString();
        }
        
        public bool Delete(int id)
        {
            Production enc = list.Find(p => p.Id == id);
            if(enc == null)
            {
                return false;
            }
            list.Remove(enc);
            List<string> dados = new List<string>();
            foreach(var item in list)
            {
                dados.Add(item.ToString());
            }
            File.WriteAllLines(Utils.path + Utils.file, dados);
            return true;
        }

        public List<Production> Listar()
        {
            return list; 
        }

        public override string ToString()
        {
            return $""" Id - {this.Id} - Data de produção - {this.ProductionDate}: - Produto - {this.Product} - Quantidade - {this.Quantity} """;
        }
    }
}
