using System;
using System.IO;

namespace UnityServe.Utils
{
    class SaveLoadLastPath
    {
        const string LAST_OPEN_FILE = "last_open.txt";

        public static void SaveLastOpenedFile(string filePath)
        {
            try
            {
                // Write the last opened directory to a text file
                using StreamWriter writer =
                    new(Path.Combine(AppContext.BaseDirectory, LAST_OPEN_FILE));
                writer.WriteLine($"last_opened={filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string LoadLastOpenedFile()
        {
            string lastOpenedFile = string.Empty;

            if (!File.Exists(LAST_OPEN_FILE))
                return lastOpenedFile;

            try
            {
                // Read the last opened directory from the text file
                using StreamReader reader =
                    new(Path.Combine(AppContext.BaseDirectory, LAST_OPEN_FILE));
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("last_opened="))
                    {
                        lastOpenedFile = line["last_opened=".Length..];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return lastOpenedFile;
        }
    }
}
