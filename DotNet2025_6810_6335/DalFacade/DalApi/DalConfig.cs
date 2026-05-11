namespace DalApi;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;

static class DalConfig
{
    internal static string s_dalName;
    internal static Dictionary<string, string> s_dalPackages;

    static DalConfig()
    {
        var configPath = FindConfigPath();
        XElement dalConfig = XElement.Load(configPath);

        s_dalName = dalConfig.Element("dal")?.Value?.Trim() ?? throw new DalConfigException("<dal> element is missing");
        var packages = dalConfig.Element("dal-packages")?.Elements() ?? throw new DalConfigException("<dal-packages> element is missing");
        s_dalPackages = packages.ToDictionary(p => p.Name.LocalName, p => p.Value.Trim());
    }

    private static string FindConfigPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "xml", "dal-config.xml");
            if (File.Exists(candidate))
                return candidate;

            candidate = Path.Combine(directory.FullName, "dal-config.xml");
            if (File.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DalConfigException("dal-config.xml file is not found");
    }
}
 
[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string msg) : base(msg) { }
    public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
}
