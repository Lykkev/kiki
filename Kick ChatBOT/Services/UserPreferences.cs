using System;
using System.IO;
using Kick_ChatBOT.Models;

namespace Kick_ChatBOT.Services
{
    public static class UserPreferences
    {
        private static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KickChatBotWin");
        private static readonly string PathsFile = Path.Combine(AppFolder, "paths.json");

        public static UserPaths LoadPaths()
        {
            try
            {
                if (!File.Exists(PathsFile))
                {
                    // Crear archivo vacío si no existe
                    var defaultPaths = new UserPaths();
                    SavePaths(defaultPaths);
                    return defaultPaths;
                }

                // Intentar cargar directamente primero
                var result = JsonStorage.ReadJsonFile(PathsFile, new UserPaths());
                
                // Si la carga falló o está vacía, intentar recuperar
                if (result == null || (string.IsNullOrEmpty(result.Kicks) && string.IsNullOrEmpty(result.Proxies)))
                {
                    var fileContent = File.ReadAllText(PathsFile);
                    
                    if (fileContent.Contains("kicks.json"))
                    {
                        var backupPaths = new UserPaths();
                        var lines = fileContent.Split('\n');
                        
                        foreach (var line in lines)
                        {
                            if (line.Contains("kicks.json") && line.Contains(":"))
                            {
                                var kicksPath = line.Split(':')[1].Replace("\"", "").Replace(",", "").Trim();
                                if (kicksPath.Contains("kicks.json"))
                                {
                                    backupPaths.Kicks = kicksPath.Replace("\\\\", "\\");
                                }
                            }
                        }
                        
                        SavePaths(backupPaths);
                        return backupPaths;
                    }
                }
                
                return result ?? new UserPaths();
            }
            catch
            {
                return new UserPaths();
            }
        }

        public static bool SavePaths(UserPaths paths)
        {
            try
            {
                if (!Directory.Exists(AppFolder)) 
                {
                    Directory.CreateDirectory(AppFolder);
                }
                
                return JsonStorage.WriteJsonFile(PathsFile, paths);
            }
            catch
            {
                return false;
            }
        }
    }
}

