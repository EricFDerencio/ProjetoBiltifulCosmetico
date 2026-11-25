public class RawMaterialService
{
    public static RawMaterial Create()
    {
        while (true)
        {
            Console.Write("Digite o nome da matéria-prima: ");
            string name = Console.ReadLine() ?? "";

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

    public static void SaveRawMaterials(List<RawMaterial> materials)
    {
        string filePath = "MateriaPrima.dat";

        List<string> lines = new List<string>();

        foreach (RawMaterial material in materials)
        {
            string line = material.ToFileString();

            if (line.Length != 43)
                throw new Exception("Tamanho de registro inválido.");

            lines.Add(line);
        }

        File.WriteAllLines(filePath, lines);
    }
}