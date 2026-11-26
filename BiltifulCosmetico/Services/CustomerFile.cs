using BiltifulCosmetico.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace BiltifulCosmetico.Services
{
    public static class CustomerFile
    {
        private static readonly string PATH = Environment.GetEnvironmentVariable("USERPROFILE") + @"\biltiful\";
        private const string FILE = "Clientes.dat";
        private static readonly string FULLPATH = Path.Combine(PATH, FILE);
        private const string RISKFILE = "Risco.dat";
        private static readonly string RISKFULLPATH = Path.Combine(PATH, RISKFILE);

        public static bool SaveRisk(string cpf)
        {
            try
            {
                cpf = Customer.NormalizarCpf(cpf);

                if (!Customer.ValidarDigitoVerificador(cpf))
                    throw new ArgumentException("CPF inválido");

                if (!FileValidation(RISKFULLPATH))
                    return false;

                // verificar no merge esse Risk exists que o rodolfo implementou
                if (IsCpfAtRisk(cpf))
                    throw new ArgumentException("CPF já cadastrado no arquivo de risco");

                File.AppendAllText(RISKFULLPATH, cpf + Environment.NewLine);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static bool RemoveRisk(string cpf)
        {
            if (!File.Exists(RISKFULLPATH))
                return false;

            try
            {
                cpf = Customer.NormalizarCpf(cpf);

                List<string> registros = new List<string>(
                    File.ReadAllLines(RISKFULLPATH));

                if (!registros.Remove(cpf))
                    return false;

                File.WriteAllLines(RISKFULLPATH, registros);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

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

        private static bool FileValidation(string fullpath)
        {
            if (File.Exists(fullpath))
                return true;
            try
            {
                if (PathValidation())
                {
                    File.Open(fullpath, FileMode.OpenOrCreate).Close();
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

        internal static bool UpdateCustomer(Customer customer)
        {
            if (!File.Exists(FULLPATH))
                return false;

            try
            {
                string[] registros = File.ReadAllLines(FULLPATH);

                string novoRegistro =
                    customer.CPF +
                    customer.Name.PadRight(50, ' ') +
                    customer.BirthDate.ToString("ddMMyyyy", CultureInfo.InvariantCulture) +
                    (char)customer.Gender +
                    customer.LastPurchase.ToString("ddMMyyyy", CultureInfo.InvariantCulture) +
                    customer.RegisterDate.ToString("ddMMyyyy", CultureInfo.InvariantCulture) +
                    (char)customer.Situation;

                if (novoRegistro.Length != 87)
                    throw new ArgumentException("Tamanho do registro inválido.");

                for (int i = 0; i < registros.Length; i++)
                {
                    if (registros[i].Length != 87)
                        throw new InvalidDataException("Registro inválido no arquivo.");

                    if (registros[i].Substring(0, 11) == customer.CPF)
                    {
                        registros[i] = novoRegistro;

                        File.WriteAllLines(FULLPATH, registros);

                        return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        internal static bool SaveCustomer(Customer customer)
        {
            if (!FileValidation(FULLPATH))
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

                File.AppendAllText(FULLPATH, registro + Environment.NewLine);

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

        public static string? FindCustomerByCPF(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "").Trim().PadLeft(11, '0');

            if (!File.Exists(FULLPATH))
                return null;

            foreach (string registro in File.ReadLines(FULLPATH))
            {
                if (registro.Length != 87)
                    throw new InvalidDataException("Registro inválido no arquivo.");

                if (registro.Substring(0, 11) == cpf)
                    return registro;
            }

            return null;
        }

        public static string[] ReadActiveCustomers()
        {
            if (!File.Exists(FULLPATH))
                return Array.Empty<string>();

            List<string> registros = new List<string>();

            foreach (string registro in File.ReadLines(FULLPATH))
            {
                if (registro.Length != 87)
                    throw new InvalidDataException("Registro inválido no arquivo.");

                if (registro[86] == 'A')
                    registros.Add(registro);
            }

            return registros.ToArray();
        }
    }
}
