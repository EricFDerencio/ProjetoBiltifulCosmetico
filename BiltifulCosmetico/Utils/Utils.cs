public static class Utils
{
    public static bool ValidateFilePath(string filePath)
    {
        return File.Exists(filePath);
    }

}
