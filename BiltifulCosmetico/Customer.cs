using BiltifulCosmetico.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace BiltifulCosmetico
{
    internal class Customer
    {
        public string CPF { get; private set; }
        public string Name { get; private set; }
        public DateOnly BirthDate { get; private set; }
        public EGender Gender { get; private set; } = EGender.NotDefined;
        public DateOnly LastPurchase { get; private set; }
        public DateOnly RegisterDate { get; set; } = new DateOnly();
        public char IsActive { get; private set; } = 'A';

        public Customer(string nome, string cpf)
        {
            SetCPF(cpf);

        }
        public bool SetCPF(string cpf)
        {
            try
            {

                cpf = new string((cpf ?? "").Where(char.IsDigit).ToArray()).PadLeft(11, '0');

                // 2. Barra CPFs maiores que 11 dígitos ou com todos os números iguais
                if (cpf.Length != 11 || cpf.Distinct().Count() == 1) return false;

                // 3. Calcula os 2 dígitos verificadores de uma vez
                for (int j = 9; j < 11; j++)
                {
                    int soma = 0;
                    for (int i = 0; i < j; i++)
                        soma += (cpf[i] - '0') * (j + 1 - i);

                    int digito = (soma * 10) % 11 % 10;
                    if ((cpf[j] - '0') != digito) return false;
                }
                CPF = cpf;
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
