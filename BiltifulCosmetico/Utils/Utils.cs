public static class Utils
{
        public static string path = @"C:\dev\";
        public static string file = "producoes.dat";
      
       public static bool ValidateFilePath(string filePath)
 {
     return File.Exists(filePath);
 }
}
