using System.IO;
using System;

using Newtonsoft.Json;
using UnityEngine;

namespace Convai.Scripts.Configuration
{
    public static class ConvaiConfigurationDataSystem
    {
        private static readonly string _editorPath = Path.Combine(Application.dataPath, "Convai", "Configuration");
        private static readonly string _runtimePath = Path.Combine(Application.persistentDataPath, "Convai", "Configuration");
        private static readonly string _fileName = "ConvaiConfigurationData.json";

        public static void SaveConfigurationData(ConvaiConfigurationDataSO configurationData)
        {
            string json = JsonConvert.SerializeObject(configurationData);
            string path = Application.isEditor ? _editorPath : _runtimePath;
            string filePath = Path.Combine(path, _fileName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            File.WriteAllText(filePath, json);
        }

        public static ConvaiConfigurationDataSO LoadConfigurationData()
        {
            string filePath;
            string json;

            if (Application.isEditor)
            {
                filePath = Path.Combine(_editorPath, _fileName);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Configuration file not found at {filePath}");
                }

                json = File.ReadAllText(filePath);
            }
            else
            {
                string runtimeFilePath = Path.Combine(_runtimePath, _fileName);
                string dataPathFilePath = Path.Combine(_editorPath, _fileName);

                if (File.Exists(runtimeFilePath))
                {
                    filePath = runtimeFilePath;
                }
                else if (File.Exists(dataPathFilePath))
                {
                    filePath = dataPathFilePath;
                    json = File.ReadAllText(filePath);

                    // Save to persistent data path
                    File.WriteAllText(runtimeFilePath, json);

                    return Convert(json);
                }
                else
                {
                    // In build, if no JSON file exists, try to load from Resources
                    ConvaiConfigurationDataSO resourceData = Resources.Load<ConvaiConfigurationDataSO>(nameof(ConvaiConfigurationDataSO));
                    if (resourceData != null)
                    {
                        // Create a copy to avoid modifying the original asset
                        ConvaiConfigurationDataSO copy = ConvaiConfigurationDataSO.Copy(resourceData);
                        EnsureEndUserId(copy);
                        return copy;
                    }

                    throw new FileNotFoundException($"Configuration file not found at {runtimeFilePath} or {dataPathFilePath}");
                }

                json = File.ReadAllText(filePath);
            }


            return Convert(json);
        }

        private static ConvaiConfigurationDataSO Convert(string json)
        {
            ConvaiConfigurationDataSO dataSO = ScriptableObject.CreateInstance<ConvaiConfigurationDataSO>();
            JsonConvert.PopulateObject(json, dataSO);
            EnsureEndUserId(dataSO);
            return dataSO;
        }

        private static void EnsureEndUserId(ConvaiConfigurationDataSO dataSO)
        {
            if (dataSO == null)
            {
                return;
            }

            string sanitizedEndUserId = SanitizeIdentifier(dataSO.EndUserId);
            if (!string.Equals(dataSO.EndUserId, sanitizedEndUserId, StringComparison.Ordinal))
            {
                dataSO.EndUserId = sanitizedEndUserId;
                SaveConfigurationData(dataSO);
            }
        }

        private static string SanitizeIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            char[] buffer = value.ToCharArray();
            int writeIndex = 0;

            for (int readIndex = 0; readIndex < buffer.Length; readIndex++)
            {
                if (!char.IsWhiteSpace(buffer[readIndex]))
                {
                    buffer[writeIndex++] = buffer[readIndex];
                }
            }

            return new string(buffer, 0, writeIndex);
        }
    }
}
