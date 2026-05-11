using System;
using System.IO;
using System.Xml.Linq;

namespace DalXml
{
    internal static class Config
    {
        private const string s_configFileName = "data-config";

        public static int ProductNum => GetNextNumber("ProductNum");

        public static int SaleNum => GetNextNumber("SaleNum");

        private static int GetNextNumber(string elementName)
        {
            XDocument doc = LoadConfigFile();
            XElement? element = doc.Root?.Element(elementName);
            if (element is null)
                throw new InvalidOperationException($"Element '{elementName}' is missing from {s_configFileName}.xml");

            if (!int.TryParse(element.Value, out int currentValue))
                throw new FormatException($"Invalid integer value for '{elementName}' in {s_configFileName}.xml.");

            int nextValue = currentValue;
            element.Value = (currentValue + 1).ToString();
            doc.Save(GetConfigFilePath());
            return nextValue;
        }

        private static XDocument LoadConfigFile()
        {
            string path = GetConfigFilePath();
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Configuration file not found: {path}");
            }

            return XDocument.Load(path);
        }

        private static string GetConfigFilePath()
        {
            return XmlHelper.Find(Path.Combine("xml", $"{s_configFileName}.xml"));
        }
    }

    internal static class XmlHelper
    {
        internal static string Find(string relativeFilePath)
        {
            var baseDir = AppContext.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDir, relativeFilePath),
                Path.Combine(baseDir, "..", "..", "..", "..", relativeFilePath)
            };

            foreach (var candidate in candidates)
            {
                var fullPath = Path.GetFullPath(candidate);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            throw new FileNotFoundException($"Configuration file not found: {relativeFilePath}");
        }
    }
}
