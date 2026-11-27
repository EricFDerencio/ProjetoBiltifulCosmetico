using System.Text.RegularExpressions;

public static class CpfValidator
{
    public static string Normalize(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF não informado.");

        // Aceita CPF com ou sem pontuação.
        if (!Regex.IsMatch(cpf, @"^[0-9.\-]+$"))
            throw new ArgumentException("CPF contém caracteres inválidos.");

        return cpf.Replace(".", "").Replace("-", "");
    }

    public static bool IsValid(string cpf)
    {
        string digits;

        try
        {
            digits = Normalize(cpf);
        }
        catch (ArgumentException)
        {
            return false;
        }

        if (digits.Length != 11)
            return false;

        // Rejeita CPFs com todos os dígitos iguais.
        if (digits.All(c => c == digits[0]))
            return false;

        int sum = 0;

        // Calcula o primeiro dígito verificador.
        for (int i = 0; i < 9; i++)
        {
            sum += (digits[i] - '0') * (10 - i);
        }

        int firstDigit = sum % 11;
        firstDigit = firstDigit < 2 ? 0 : 11 - firstDigit;

        if (firstDigit != digits[9] - '0')
            return false;

        sum = 0;

        // Calcula o segundo dígito verificador.
        for (int i = 0; i < 10; i++)
        {
            sum += (digits[i] - '0') * (11 - i);
        }

        int secondDigit = sum % 11;
        secondDigit = secondDigit < 2 ? 0 : 11 - secondDigit;

        return secondDigit == digits[10] - '0';
    }
}
