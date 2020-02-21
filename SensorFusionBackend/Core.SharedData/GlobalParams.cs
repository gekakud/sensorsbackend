using System.IO;

namespace Core.Common
{
    public static class GlobalParams
    {
        #region Serilog

        public const string LogFolderPath = @"C:\Logs";
        public static string LogFilePath = Path.Combine(LogFolderPath, "log-{Date}.txt");

        #endregion

    }
}
