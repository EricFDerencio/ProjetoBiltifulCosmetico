using BiltifulCosmetico.Classes;
using BiltifulCosmetico.Services;

public class RiskFileRepository
{
    private readonly string _filePath;

    public RiskFileRepository(string? filePath = null)
    {
        _filePath = filePath ?? CustomerFile.RISKFULLPATH;
    }

    public bool IsCpfAtRisk(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF não informado.", nameof(cpf));

        cpf = Customer.NormalizarCpf(cpf);

        if (!Customer.ValidarDigitoVerificador(cpf))
            throw new ArgumentException("CPF inválido.", nameof(cpf));

        if (!File.Exists(_filePath))
            return false;

        foreach (string registro in File.ReadLines(_filePath))
        {
            if (registro.Length != 11)
            {
                throw new InvalidDataException(
                    "Registro inválido no arquivo Risco.dat."
                );
            }

            if (!registro.All(char.IsAsciiDigit))
            {
                throw new InvalidDataException(
                    "O arquivo Risco.dat contém um CPF inválido."
                );
            }

            if (registro == cpf)
                return true;
        }

        return false;
    }
}
