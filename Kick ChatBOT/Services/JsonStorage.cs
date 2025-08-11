using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Kick_ChatBOT.Models;

namespace Kick_ChatBOT.Services
{
    public static class JsonStorage
    {
        public static T ReadJsonFile<T>(string path, T @default = default(T))
        {
            try
            {
                // Si no hay ruta, usar default
                if (string.IsNullOrEmpty(path))
                {
                    return @default;
                }

                if (!File.Exists(path)) 
                {
                    return @default;
                }

                var fileInfo = new FileInfo(path);
                if (fileInfo.Length == 0)
                {
                    return @default;
                }

                // Leer el archivo de manera más robusta
                byte[] fileBytes = File.ReadAllBytes(path);
                
                // Detectar y remover BOM UTF-8
                if (fileBytes.Length >= 3 && fileBytes[0] == 0xEF && fileBytes[1] == 0xBB && fileBytes[2] == 0xBF)
                {
                    byte[] withoutBom = new byte[fileBytes.Length - 3];
                    Array.Copy(fileBytes, 3, withoutBom, 0, withoutBom.Length);
                    fileBytes = withoutBom;
                }
                
                var jsonText = Encoding.UTF8.GetString(fileBytes);
                
                // Limpiar caracteres de control invisibles
                jsonText = jsonText.Trim('\0', '\r', '\n', '\t', ' ');
                
                // Verificar que el archivo empiece correctamente
                if (typeof(T) == typeof(List<Account>) && !jsonText.StartsWith("["))
                {
                    return @default;
                }

                // Configuración simple y permisiva para Newtonsoft.Json
                var settings = new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DateParseHandling = DateParseHandling.None,
                    FloatParseHandling = FloatParseHandling.Double
                };
                
                var result = JsonConvert.DeserializeObject<T>(jsonText, settings);
                return result != null ? result : @default;
            }
            catch
            {
                return @default;
            }
        }

        public static bool WriteJsonFile<T>(string path, T obj)
        {
            try
            {
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText(path, json, Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

