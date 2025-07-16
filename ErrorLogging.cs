using Decal.Adapter;
using System;
using System.IO;

namespace WaynesWorld
{
    internal class ErrorLogging
    {
        private static readonly object _fileLock = new object();
        private static readonly string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + "Decal Plugins" + "\\" + "WaynesWorld" + "\\" + "AutoLootStateMachine.txt";


        internal static void LogError(string logFile, Exception ex)
        {
            using (StreamWriter sw = new StreamWriter(logFile, true))
            {
                sw.WriteLine("============================================================================");
                sw.WriteLine(DateTime.Now.ToString());
                sw.WriteLine("Error: " + ex.Message);
                sw.WriteLine("Source: " + ex.Source);
                sw.WriteLine("Stack: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    sw.WriteLine("Inner: " + ex.InnerException.Message);
                    sw.WriteLine("Inner Stack: " + ex.InnerException.StackTrace);
                }
                sw.WriteLine("============================================================================");
                sw.WriteLine("");
                sw.Close();
            }
        }

        /*
             * 0 - No logging
             * 1 - Log to file only
             * 2 - Log to file and chat
             */
        internal static void log(string message, int log_level)
        {
            
            string textToAppend = $"[LootFSM: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {message}";

            if (log_level > 0)
            {
                lock (_fileLock)
                {
                    // Ensure the directory exists before appending to the file
                    string directoryPath = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    // Append the text to the file
                    File.AppendAllText(filePath, textToAppend + "\n");
                }
                
            }
            if (log_level > 1)
            {
                CoreManager.Current.Actions.AddChatText(textToAppend, 5);
            }
            return;
        }
    }
}