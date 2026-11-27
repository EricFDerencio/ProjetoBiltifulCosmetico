public class PurchaseFile
{
    //caminho definido como padrao pelo grupo
    public string path = Environment.GetEnvironmentVariable("USERPROFILE") + @"\biltiful\";
    public string file = "Compra.dat";

    public bool FileValidation()
    {
        try
        {
            Directory.CreateDirectory(path);
            if (!File.Exists(path + file))
                File.Create(path + file).Close();

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }

    }

    public void Save(Purchase p)
    {
        if (!FileValidation())
            return;

        var lines = new List<string>();
        var reader = new StreamReader(path + file);

        string aux;
        while ((aux = reader.ReadLine()) != null)
        {
            lines.Add(aux);
        }
        reader.Close();

        lines.Add(p.ToFileLine());

        var writer = new StreamWriter(path + file);
        foreach (var line in lines)
        {
            writer.WriteLine(line);
        }
        writer.Close();
    }

    public int GetNextId()
    {
        if (!FileValidation())
        {
            return 0;
        }

        var lines = new List<string>();
        var reader = new StreamReader(path + file);

        string aux;
        while ((aux = reader.ReadLine()) != null)
        {
            lines.Add(aux);
        }
        reader.Close();

        if (lines.Count == 0)
            return 1;

        //pega a ultima linha, seleciona apenas os 5 primeiros caracteres e faz o parseInt
        string lastLine = lines[lines.Count - 1];
        string lastIdText = lastLine.Substring(0, 5); //"recorta a string"
        int lastId = int.Parse(lastIdText);

        return lastId + 1;
    }
}
