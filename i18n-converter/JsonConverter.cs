using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace I18nConverter
{
    public class JsonConverter
    {
        private readonly Logger _logger;

        public JsonConverter(Logger logger)
        {
            _logger = logger;
        }
        
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> FromJson(string baseDirectory)
        {
            _logger.LogVerbose("Reading json structure...");
            
            var languageDirectories = Directory.GetDirectories(baseDirectory);
            _logger.LogVerbose(string.Join(", ", languageDirectories));

            var languages = languageDirectories.Select(s => Path.GetFileName(s)!).ToList();
            _logger.LogVerbose(string.Join(", ", languages));

            var files = languageDirectories
                .SelectMany(d => Directory.GetFiles(d, "*.json"))
                .Select(s => Path.GetFileName(s)!)
                .Distinct()
                .ToList();
            _logger.LogVerbose(string.Join(", ", files));

            var namespaces = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = {new I18nJsonConverter()}
            };
            foreach (var file in files)
            {
                var namespaceDictionary = new Dictionary<string, Dictionary<string, string>>();
                foreach (var language in languages)
                {
                    var path = Path.Combine(baseDirectory, language, file);
                    if (File.Exists(path))
                    {
                        _logger.LogVerbose($"Reading {path}...");
                        var content = File.ReadAllText(path);
                        var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(content, jsonSerializerOptions)!;

                        namespaceDictionary.Add(language, dictionary);
                    }
                }

                namespaces.Add(Path.GetFileNameWithoutExtension(file), namespaceDictionary);
            }

            return namespaces;
        }

        public void ToJson(Dictionary<string, Dictionary<string, Dictionary<string, string>>> namespaces,
            string outPath)
        {
            _logger.LogVerbose("Writing output...");
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = {new I18nJsonConverter()},
                WriteIndented = true
            };
            // ReSharper disable once InconsistentNaming
            foreach (var (i18nNamespace, namespaceDictionary) in namespaces)
            {
                foreach (var (language, languageDictionary) in namespaceDictionary)
                {
                    var filePath = Path.Combine(outPath, language, i18nNamespace + ".json");
                    if (!languageDictionary.Any())
                    {
                        File.Delete(filePath);
                        continue;
                    }
                    
                    var json = JsonSerializer.Serialize(languageDictionary, jsonSerializerOptions);
                    
                    if (!Directory.Exists(outPath))
                    {
                        Directory.CreateDirectory(outPath);
                    }
                    
                    var languageDirectory = Path.Combine(outPath, language);
                    if (!Directory.Exists(languageDirectory))
                    {
                        Directory.CreateDirectory(languageDirectory);
                    }

                    File.WriteAllText(filePath, json);
                }
            }
        }
    }
}