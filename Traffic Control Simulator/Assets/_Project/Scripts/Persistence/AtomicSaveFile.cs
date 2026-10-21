using System;
using System.IO;

public static class AtomicSaveFile
{
    public static bool TryWrite(string path, string payload, out string error)
    {
        error = null;
        try
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            string temporary = path + ".tmp";
            File.WriteAllText(temporary, payload);
            if (File.Exists(path)) File.Replace(temporary, path, path + ".bak");
            else File.Move(temporary, path);
            return true;
        }
        catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException || exception is NotSupportedException || exception is ArgumentException)
        { error = exception.Message; return false; }
    }
}
