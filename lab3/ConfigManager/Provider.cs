using System.IO;

namespace ConfigManager
{
    public class Provider<T> where T : class
    {
        public readonly IConfigurationParser<T> configurationParser;

        public Provider(string path)
        {
            switch (Path.GetExtension(path))
            {
                case ".xml":
                    configurationParser = new XmlParser<T>(path);
                    break;

                case ".json":
                    configurationParser = new JsonParser<T>(path);
                    break;
            }
        }
    }
}