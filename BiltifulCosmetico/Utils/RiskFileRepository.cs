public class RiskFileRepository
{
    private readonly string _filePath;

    public RiskFileRepository(string filePath = "Risco.dat")
    {
        _filePath = filePath;
    }

    public bool IsCpfAtRisk(string cpf)
    {
        if (!CpfValidator.IsValid(cpf))
            throw new ArgumentException("CPF inválido.");

        string normalizeCpf = CpfValidator.Normalize(cpf);

        if (!File.Exists(_filePath))
            throw new FileNotFoundException("Arquivo Risco.dat não encontrado.", _filePath);

        foreach (string line in File.ReadLines(_filePath))
        {
            if (line.Length != 11 || !line.All(char.IsAsciiDigit))
                throw new InvalidDataException("O arquivo Risco.dat possui um registro inválido.");
            if (line == normalizeCpf)
                return true;
        }
        return false;
    }

}