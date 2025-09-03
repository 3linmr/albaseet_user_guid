namespace Shared.API.Helpers
{
    public static class ReadFile
    {
        public static string ReadScripts( string fileName)
        {
            return ReadFileToEnd("scripts", fileName);
        }

        public static string ReadFileToEnd(string folderName,string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName, fileName);
            using StreamReader sr = new StreamReader(filePath);
            string text = sr.ReadToEnd();
            return text;
        }
    }
}
