using ConfigurationProvider;
using System;
using System.IO;

namespace Lab4
{
    public class DataManager
    {
        private Data configOptions;
        private DBApplicationInsights insights;
        public DataManager(DBApplicationInsights insights, Data configOptions)
        {
            this.insights = insights;
            this.configOptions = configOptions;
        }

        public void Transfer()
        {
            string connectionString = configOptions.ConnectionString;
            DBAdventure dbAdventure = new DBAdventure(connectionString);
            string personPath = AppDomain.CurrentDomain.BaseDirectory;
            personPath = Path.Combine(personPath.Substring(0, personPath.Length - 20), "Product");
            DirectoryInfo personFolder = new DirectoryInfo(personPath);
            if (!personFolder.Exists)
            {
                personFolder.Create();
            }
            for (int begin = 0, end = 1000, idName = 1; begin < count; begin += 1000, end += 1000, idName++)
            {
                string fileName = $"Product_{idName}.xml";
                string fullFileName = Path.Combine(personPath, fileName);
                dbAdventure.GetPerson(fullFileName, insights);
                File.Copy(fullFileName, Path.Combine(configOptions.SourceDirectory, fileName));
                File.Copy(Path.ChangeExtension(fullFileName, "xsd"),
                            Path.Combine(configOptions.SourceDirectory, Path.ChangeExtension(fileName, "xsd")));
            }
            insights.AddAction("Success: Main. Data transfer to DataManager", DateTime.Now);
        }
    }
}
