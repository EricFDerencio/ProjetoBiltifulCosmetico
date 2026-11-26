public static class RawMaterialFile
{

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
    public static void SaveRawMaterials(List<RawMaterial> materials,string filepath)
    {
        
        List<string> lines = new List<string>();

        var directory = Path.GetDirectoryName(filepath);

        if (!Directory.Exists(directory) && directory is not null)
            Directory.CreateDirectory(directory);
        else
            throw new ArgumentException("Caminho Invalido");

        foreach (RawMaterial material in materials)
        {
            string line = material.ToFileString();
            if (line.Length != 43)
                throw new Exception("Tamanho de registro inválido.");
            lines.Add(line);
        }

        File.WriteAllLines(filepath, lines);
    }
    public static RawMaterial FromFileString(string line)
    {
        string id = line.Substring(0, 6);

        string name = line.Substring(6, 20).Trim();
        DateTime lastBuy = DateTime.ParseExact(
            line.Substring(26, 8),
            "ddMMyyyy",
            null
        );

        DateTime dateOfRegister = DateTime.ParseExact(
            line.Substring(34, 8),
            "ddMMyyyy",
            null
        );

        ESituation situation = (ESituation)line[42];

        return new RawMaterial(id, name, lastBuy, dateOfRegister, situation);

    }
    public static List<RawMaterial> LoadRawMaterials()
    {
        string filepath = Environment.GetEnvironmentVariable("USERPROFILE")+ @"\biltiful\MateriaPrima.dat";

        List<RawMaterial> materials = new List<RawMaterial>();
        try
        {
            if (!Utils.ValidateFilePath(filepath))
                return materials;

            foreach (string line in File.ReadAllLines(filepath))
            {
                if (line.Length != 43)
                    throw new Exception("Registro com tamanho inválido.");

                materials.Add(FromFileString(line));
            }
            return materials;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao carregar os Materiais: {e.Message}");
            return materials;
        }
    }

    public static List<RawMaterial> SearchMaterials(List<RawMaterial> materials,string name)
    {
        return materials.FindAll(
            material => material.Name.Contains(name, StringComparison.OrdinalIgnoreCase)
        );
    }

}