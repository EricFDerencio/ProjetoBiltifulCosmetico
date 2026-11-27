using System;
using System.Collections.Generic;
using System.Text;

namespace BiltifulCosmetico.Dados
{
    internal class ProductionFiles
    {
        public static string caminho = Environment.GetEnvironmentVariable("USERPROFILE") + @"\biltiful\ProdutcionTeste4.dat";

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
            if (string.IsNullOrWhiteSpace(p))
                return false;

            try
            {
                if (File.Exists(p))
                    return true;

                string? directory = Path.GetDirectoryName(p);

                if (string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Create(p).Close();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }

}
