
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public static class RawMaterialFile
{
    public static readonly string FilePath = Path.Combine(
        Environment.GetEnvironmentVariable("USERPROFILE") ?? string.Empty,
        "biltiful",
        "MateriaPrima.dat");

    public static RawMaterial Create()
    {
        while (true)
        {
            Console.Write("Digite o nome da matéria-prima: ");
            string name = Console.ReadLine() ?? string.Empty;

            try
            {
                return new RawMaterial(name);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                Console.WriteLine("Tente novamente.\n");
            }
        }
    }

    public static void SaveRawMaterials(List<RawMaterial> materials, string filepath)
    {
        List<string> lines = new List<string>();

        string? directory = Path.GetDirectoryName(filepath);

        if (string.IsNullOrEmpty(directory))
            throw new ArgumentException("Caminho inválido.");

        Directory.CreateDirectory(directory);

        foreach (RawMaterial material in materials)
        {
            if (material == null)
                throw new InvalidDataException(
                    "A lista contém uma matéria-prima inválida."
                );

            string line = material.ToFileString();

            if (line.Length != RecordLength)
                throw new InvalidDataException(
                    "Tamanho de registro inválido."
                );

            lines.Add(line);
        }

        File.WriteAllLines(filepath, lines);
    }

    public static RawMaterial FromFileString(string line)
    {
        if (string.IsNullOrEmpty(line) || line.Length != RecordLength)
            throw new InvalidDataException(
                "Registro de matéria-prima inválido."
            );

        string id = line.Substring(0, 6);
        string name = line.Substring(6, 20).Trim();

        DateTime lastBuy = DateTime.ParseExact(
            line.Substring(26, 8),
            "ddMMyyyy",
            CultureInfo.InvariantCulture
        );

        DateTime dateOfRegister = DateTime.ParseExact(
            line.Substring(34, 8),
            "ddMMyyyy",
            CultureInfo.InvariantCulture
        );

        char situationCode = line[42];

        if (situationCode != 'A' && situationCode != 'I')
            throw new InvalidDataException(
                "Situação da matéria-prima inválida."
            );

        return new RawMaterial(id, name, lastBuy, dateOfRegister, situation);
    }

    public static List<RawMaterial> LoadRawMaterials()
    {
        List<RawMaterial> materials = new List<RawMaterial>();
        try
        {
            if (!Utils.ValidateFilePath(FilePath))
                return materials;

            foreach (string line in File.ReadAllLines(FilePath))
            {
                if (line.Length != RecordLength)
                    throw new InvalidDataException(
                        "Registro com tamanho inválido."
                    );

                materials.Add(FromFileString(line));
            }
        }
        catch (Exception ex) when (
            ex is IOException ||
            ex is UnauthorizedAccessException ||
            ex is FormatException ||
            ex is ArgumentException
        )
        {
            Console.WriteLine(
                $"Ocorreu um erro ao carregar as matérias-primas: {ex.Message}"
            );

            throw;
        }

        return materials;
    }

    public static List<RawMaterial> SearchMaterials(List<RawMaterial> materials, string name)
    {
        ArgumentNullException.ThrowIfNull(materials);

        if (string.IsNullOrWhiteSpace(name))
            return new List<RawMaterial>();

        return materials.FindAll(
            material => material.Name.Contains(
                name,
                StringComparison.OrdinalIgnoreCase
            )
        );
    }

    public static RawMaterial? FindById(List<RawMaterial> materials, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return materials.FirstOrDefault(
            material => material.Id.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase)
        );
    }
}
