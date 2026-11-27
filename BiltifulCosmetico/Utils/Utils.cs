public static class Utils
{
    public static string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "biltiful"
    ) + Path.DirectorySeparatorChar;

    public static string file = "producoes.dat";

    public static bool ValidateFilePath(string filePath)
    {
        return File.Exists(filePath);
    }
}
