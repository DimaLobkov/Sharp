using System;
using System.Data;
using System.IO;
using System.Data.SqlClient;

namespace Lab4
{
    public class DBAdventure
    {
        private readonly string connectionString;
        public DBAdventure(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void GetPerson(string path, DBApplicationInsights insights)
        {
            string sqlExpression = "Information";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;

                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet ds = new DataSet("Product");
                    DataTable dt = new DataTable("Product");
                    ds.Tables.Add(dt);
                    adapter.Fill(ds.Tables["Product"]);
                    ds.WriteXml(path);
                    ds.WriteXmlSchema(Path.ChangeExtension(path, "xsd"));
                    insights.AddInTable("Success: GetProduct", DateTime.Now);
                }
                catch (Exception e)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorFile.txt"), true))
                    {
                        sw.WriteLine($"Error in method DBAdventure.GetProduct():{e.Message} \t {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    }
                    insights.AddInTable("Error: GetProduct", DateTime.Now);
                    transaction.Rollback();
                }
            }
        }
    }
}