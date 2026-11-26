using System.Globalization;
using BiltifulCosmetico.Services;
public class RiskFileRepository
{
    public bool IsCpfAtRisk(string cpf)
    {
        if (!CpfValidator.IsValid(cpf))
            throw new ArgumentException("CPF inválido.");

        string normalizeCpf = CpfValidator.Normalize(cpf);

        if (!File.Exists(CustomerFile.RISKFULLPATH))
            throw new FileNotFoundException("Arquivo Risco.dat não encontrado.", CustomerFile.RISKFULLPATH);

        foreach (string line in File.ReadLines(CustomerFile.RISKFULLPATH))
        {
            if (line.Length != 11 || !line.All(char.IsAsciiDigit))
                throw new InvalidDataException("O arquivo Risco.dat possui um registro inválido.");
            if (line == normalizeCpf)
                return true;
        }
        return false;
    }
}