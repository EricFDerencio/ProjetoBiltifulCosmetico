using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace BiltifulCosmetico
{
    public static class CustomerFile
    {
        private static readonly string PATH = Environment.GetEnvironmentVariable("USERPROFILE") + @"\biltiful\";
        private const string FILE = "Clientes.dat";
        private static readonly string FULLPATH = Path.Combine(PATH, FILE);


        private static bool PathValidation()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(PATH));
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

        private static bool FileValidation()
        {
            if (File.Exists(FULLPATH))
                return true;
            try
            {
                if (PathValidation())
                {
                    File.Create(Path.Combine(FULLPATH)).Close();
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

        internal static bool SaveCustomer(Customer customer)
        {
            if (!FileValidation())
                return false;

            try
            {
                if (CPFExists(customer.CPF))
                    throw new ArgumentException("CPF já cadastrado.");

                string registro =
                    customer.CPF +
                    customer.Name.PadRight(50, ' ') +
                    customer.BirthDate.ToString("ddMMyyyy", CultureInfo.InvariantCulture) +
                    (char)customer.Gender +
                    customer.LastPurchase.ToString("ddMMyyyy", CultureInfo.InvariantCulture) +
                    customer.RegisterDate.ToString("ddMMyyyy", CultureInfo.InvariantCulture) +
                    (char)customer.Situation;

                if (registro.Length != 87)
                    throw new ArgumentException("Tamanho do registro inválido.");

                if (registro.Contains('\n') || registro.Contains('\r'))
                    throw new ArgumentException("Registro contém quebra de linha.");

                File.AppendAllText(Path.Combine(PATH, FILE), registro + Environment.NewLine);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        internal static bool CPFExists(string cpf)
        {
            if (!File.Exists(FULLPATH))
                return false;

            foreach (string linha in File.ReadLines(FULLPATH))
            {
                if (linha.Length != 87)
                    throw new InvalidDataException("Registro inválido no arquivo.");

                if (linha.Substring(0, 11) == cpf)
                    return true;
            }

            return false;
        }
    }
}
