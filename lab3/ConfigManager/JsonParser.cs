using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text.Json;

namespace ConfigManager
{
    class JsonParser<T> : IConfigurationParser<T> where T : class
    {
        private readonly string jsonPath;

        public JsonParser(string jsonPath)
        {
            this.jsonPath = jsonPath;
        }

        public T Parse()
        {
            using (var fileStream = new FileStream(jsonPath, FileMode.OpenOrCreate))
            {
                using (var document = JsonDocument.Parse(fileStream))
                {
                    var element = document.RootElement;

                    if (typeof(T).GetProperties().First().Name
                        != element.EnumerateObject().First().Name)
                    {
                        element = element.GetProperty(typeof(T).Name);
                    }
                    try
                    {
                        return JsonSerializer.Deserialize<T>(element.GetRawText());
                    }
                    catch (Exception ex)
                    {
                        using (var streamWriter = new StreamWriter(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorMessage.txt"),
                            true, Encoding.Default))
                        {
                            streamWriter.WriteLine("Json file deserialization error: " + ex.Message);
                        }

                        return null;
                    }

                }
            }
        }
    }
}