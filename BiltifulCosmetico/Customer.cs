using BiltifulCosmetico.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BiltifulCosmetico
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
        public void Activate()
        {
            Situation = ESituation.Active;
        }

        public void Deactivate()
        {
            Situation = ESituation.Inactive;
        }

        public void UpdateLastSell()
        {
            LastPurchase = DateOnly.FromDateTime(DateTime.Today);
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

            string NormalizarCpf(string cpf)
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

            bool ValidarDigitoVerificador(string cpf)
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

