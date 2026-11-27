using BiltifulCosmetico.Services;
using BiltifulCosmetico.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BiltifulCosmetico.Classes
{
    internal class Customer
    {
        public string CPF { get; private set; }
        public string Name { get; private set; }
        public DateOnly BirthDate { get; private set; }
        public EGender Gender { get; private set; } = EGender.NotDefined;
        public DateOnly LastPurchase { get; private set; } = DateOnly.MinValue;
        public DateOnly RegisterDate { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
        public ESituation Situation { get; private set; } = ESituation.Active;

        public Customer(string nome, string cpf, string birth, char gender)
        {
            SetCPF(cpf);
            SetName(nome);
            SetBirthDate(birth);
            SetGender(gender);
        }

        public Customer(string registro)
        {
            if (registro.Length != 87)
                throw new ArgumentException("Registro inválido.");

            SetCPF(registro.Substring(0, 11));
            SetName(registro.Substring(11, 50).TrimEnd());
            SetBirthDate(registro.Substring(61, 8));
            SetGender(registro[69]);

            LastPurchase = DateOnly.ParseExact(
                registro.Substring(70, 8),
                "ddMMyyyy",
                CultureInfo.InvariantCulture);

            RegisterDate = DateOnly.ParseExact(
                registro.Substring(78, 8),
                "ddMMyyyy",
                CultureInfo.InvariantCulture);

            switch (registro[86])
            {
                case 'A':
                    Activate();
                    break;

                case 'I':
                    Deactivate();
                    break;

                default:
                    throw new ArgumentException("Situação inválida.");
            }
        }

        public static void MenuCliente()
        {
            while (true)
            {
                Console.WriteLine("""

            === Clientes ===
            1 - Cadastrar
            2 - Consultar por CPF
            3 - Editar
            4 - Ativar ou inativar
            5 - Listar todos
            6 - Risco
            0 - Sair
            """);

                Console.WriteLine("Qual opção você deseja?");
                string? opcao = Console.ReadLine()?.Trim();

                if (opcao is null or "0")
                    return;

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            {
                                Console.WriteLine("Qual é o CPF?");
                                string cpf = Console.ReadLine() ?? "";

                                Console.WriteLine("Qual é o nome?");
                                string nome = Console.ReadLine() ?? "";

                                Console.WriteLine("Qual é a data de nascimento (ddMMyyyy)?");
                                string nascimento = Console.ReadLine()?.Trim() ?? "";

                                Console.WriteLine("Qual é o sexo (M/F)?");
                                string sexo = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";

                                var cliente = new Customer(nome, cpf, nascimento, sexo[0]);

                                Console.WriteLine(CustomerFile.SaveCustomer(cliente)
                                    ? "Cliente cadastrado."
                                    : "Não foi possível cadastrar o cliente.");
                                break;
                            }

                        case "2":
                            {
                                Console.WriteLine("Qual é o CPF do cliente?");
                                string cpf = Console.ReadLine() ?? "";

                                string? registro = CustomerFile.FindCustomerByCPF(cpf);

                                if (registro is null)
                                    Console.WriteLine("Cliente não encontrado.");
                                else
                                    Console.WriteLine(new Customer(registro));

                                break;
                            }

                        case "3":
                            {
                                Console.WriteLine("Qual é o CPF do cliente que deseja editar?");
                                string cpf = Console.ReadLine() ?? "";
                                string? registro = CustomerFile.FindCustomerByCPF(cpf);

                                if (registro is null)
                                {
                                    Console.WriteLine("Cliente não encontrado.");
                                    break;
                                }

                                var cliente = new Customer(registro);
                                Console.WriteLine(cliente);
                                Console.WriteLine("Pressione Enter para manter o valor atual.");

                                Console.WriteLine("Qual é o novo nome?");
                                string nome = Console.ReadLine()?.Trim() ?? "";

                                Console.WriteLine("Qual é a nova data de nascimento (ddMMyyyy)?");
                                string nascimento = Console.ReadLine()?.Trim() ?? "";

                                Console.WriteLine("Qual é o novo sexo (M/F)?");
                                string sexo = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";

                                if (sexo is not ("" or "M" or "F"))
                                {
                                    Console.WriteLine("Informe M ou F.");
                                    break;
                                }

                                if (nome != "")
                                    cliente.SetName(nome);

                                if (nascimento != "")
                                    cliente.SetBirthDate(nascimento);

                                if (sexo != "")
                                    cliente.SetGender(sexo[0]);

                                Console.WriteLine(CustomerFile.UpdateCustomer(cliente)
                                    ? "Cliente atualizado."
                                    : "Não foi possível atualizar o cliente.");
                                break;
                            }

                        case "4":
                            {
                                Console.WriteLine("Qual é o CPF do cliente?");
                                string cpf = Console.ReadLine() ?? "";
                                string? registro = CustomerFile.FindCustomerByCPF(cpf);

                                if (registro is null)
                                {
                                    Console.WriteLine("Cliente não encontrado.");
                                    break;
                                }

                                var cliente = new Customer(registro);

                                Console.WriteLine("Qual é a nova situação? A para ativo ou I para inativo.");
                                string? situacao = Console.ReadLine()?.Trim().ToUpperInvariant();

                                if (situacao == "A")
                                    cliente.Activate();
                                else if (situacao == "I")
                                    cliente.Deactivate();
                                else
                                {
                                    Console.WriteLine("Situação inválida.");
                                    break;
                                }

                                Console.WriteLine(CustomerFile.UpdateCustomer(cliente)
                                    ? "Situação atualizada."
                                    : "Não foi possível atualizar a situação.");
                                break;
                            }

                        case "5":
                            {
                                string[] registros = CustomerFile.ReadAllCustomers();

                                if (registros.Length == 0)
                                {
                                    Console.WriteLine("Nenhum cliente cadastrado.");
                                    break;
                                }

                                for (int i = 0; i < registros.Length; i++)
                                {
                                    var cliente = new Customer(registros[i]);
                                    Console.WriteLine($"\nCliente {i + 1} de {registros.Length}");
                                    Console.WriteLine(cliente);
                                }

                                break;
                            }

                        case "6":
                            MenuRisco();
                            break;

                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
                catch (Exception erro) when (
                    erro is ArgumentException ||
                    erro is FormatException ||
                    erro is IOException ||
                    erro is InvalidDataException ||
                    erro is UnauthorizedAccessException)
                {
                    Console.WriteLine(erro.Message);
                }
            }
        }

        private static void MenuRisco()
        {
            var repositorio = new RiskFileRepository();

            while (true)
            {
                Console.WriteLine("""

            === Risco ===
            1 - Inserir CPF
            2 - Retirar CPF
            3 - Consultar CPF
            0 - Voltar
            """);

                Console.WriteLine("Qual opção você deseja?");
                string? opcao = Console.ReadLine()?.Trim();

                if (opcao is null or "0")
                    return;

                try
                {
                    switch (opcao)
                    {
                        case "1":
                            {
                                Console.WriteLine("Qual CPF deseja inserir na lista de risco?");
                                string cpf = Console.ReadLine() ?? "";

                                Console.WriteLine(CustomerFile.SaveRisk(cpf)
                                    ? "CPF inserido na lista de risco."
                                    : "Não foi possível inserir o CPF.");
                                break;
                            }

                        case "2":
                            {
                                Console.WriteLine("Qual CPF deseja retirar da lista de risco?");
                                string cpf = Console.ReadLine() ?? "";

                                Console.WriteLine(CustomerFile.RemoveRisk(cpf)
                                    ? "CPF retirado da lista de risco."
                                    : "Não foi possível retirar o CPF.");
                                break;
                            }

                        case "3":
                            {
                                Console.WriteLine("Qual CPF deseja consultar?");
                                string cpf = Console.ReadLine()?.Trim() ?? "";

                                bool estaEmRisco = repositorio.IsCpfAtRisk(cpf);

                                Console.WriteLine(estaEmRisco
                                    ? "CPF consta na lista de risco."
                                    : "CPF não consta na lista de risco.");
                                break;
                            }

                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("O arquivo Risco.dat ainda não foi criado.");
                }
                catch (Exception erro) when (
                    erro is ArgumentException ||
                    erro is IOException ||
                    erro is InvalidDataException ||
                    erro is UnauthorizedAccessException)
                {
                    Console.WriteLine(erro.Message);
                }
            }
        }

        public void Activate()
        {
            Situation = ESituation.Active;
        }

        public void Deactivate()
        {
            Situation = ESituation.Inactive;
        }

        public bool SetGender(char gender)
        {
            switch (char.ToUpperInvariant(gender))
            {
                case 'M':
                    Gender = EGender.Male;
                    return true;

                case 'F':
                    Gender = EGender.Female;
                    return true;   

                default:
                    Gender = EGender.NotDefined;
                    return true;
            }
        }

        public bool IsAdult()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            return BirthDate.AddYears(18) <= today;
        }

        public bool SetBirthDate(string birth)
        {
            if (!DateOnly.TryParseExact(
                birth,
                "ddMMyyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly birthDate))
            {
                throw new ArgumentException("Data de nascimento inválida", nameof(birth));
            }

            if (birthDate > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException("Data de nascimento não pode ser futura", nameof(birth));
            }

            BirthDate = birthDate;
            return true;
        }

        public bool SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome não pode ser vazio ou nulo", nameof(name));

            name = name.Trim();

            if (name.Count() > 50)
                throw new ArgumentException("Nome não pode ter mais que 50 caracteres", nameof(name));

            Name = name;
            return true;

        }

        private bool SetCPF(string cpf)
        {
            cpf = NormalizarCpf(cpf);

            if (ValidarDigitoVerificador(cpf))
            {
                CPF = cpf;
                return true;
            }
            else
            {
                throw new ArgumentException("CPF não é válido", nameof(cpf));
            }
        }

        public static string NormalizarCpf(string cpf)
        {
            if (cpf == null)
                throw new ArgumentNullException(nameof(cpf), "CPF não pode ser nulo");

            cpf = cpf.Replace(".", "");
            cpf = cpf.Replace("-", "");
            cpf = cpf.Trim();

            if (cpf.Length > 11)
                throw new ArgumentException("CPF não pode possuir mais de 11 dígitos.", nameof(cpf));

            if (cpf.Length == 0)
                throw new ArgumentException("CPF não pode estar vazio", nameof(cpf));


            for (int i = 0; i < cpf.Length; i++)
            {
                if (cpf[i] < '0' || cpf[i] > '9')
                    throw new ArgumentException("CPF contém caracteres inválidos.", nameof(cpf));
            }

            cpf = cpf.PadLeft(11, '0');

            if (cpf.Distinct().Count() == 1)
                throw new ArgumentException("CPF contém todos os dígitos iguais.", nameof(cpf));

            return cpf;
        }

        public static bool ValidarDigitoVerificador(string cpf)
        {
            for (int quantidadeDigitos = 9; quantidadeDigitos <= 10; quantidadeDigitos++)
            {
                int soma = 0;
                int peso = quantidadeDigitos + 1;

                for (int i = 0; i < quantidadeDigitos; i++)
                {
                    int numero = int.Parse(cpf[i].ToString());

                    soma += numero * peso;
                    peso--;
                }

                int resto = soma % 11;
                int digitoCalculado;

                if (resto < 2)
                    digitoCalculado = 0;
                else
                    digitoCalculado = 11 - resto;

                int digitoOriginal = int.Parse(cpf[quantidadeDigitos].ToString());

                if (digitoCalculado != digitoOriginal)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"CPF: {CPF}\n" +
                   $"Nome: {Name}\n" +
                   $"Data de nascimento: {BirthDate:dd/MM/yyyy}\n" +
                   $"Sexo: {Gender}\n" +
                   $"Última compra: {(LastPurchase == DateOnly.MinValue
                       ? "Nenhuma compra"
                       : LastPurchase.ToString("dd/MM/yyyy"))}\n" +
                   $"Data de cadastro: {RegisterDate:dd/MM/yyyy}\n" +
                   $"Situação: {Situation}";
        }
    }
}

