using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4
{
    public class DBApplicationInsights
    {
        private readonly string connectionString;
        public DBApplicationInsights(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddInTable(string message, DateTime time)
        {
            string sqlExpression = "AddInTable";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Transaction = transaction;

                try
                {
                    SqlParameter messageParam = new SqlParameter { ParameterName = "@Message", Value = message };
                    SqlParameter timeParam = new SqlParameter { ParameterName = "@Time", Value = time };
                    command.Parameters.Add(messageParam);
                    command.Parameters.Add(timeParam);

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorFile.txt"), true))
                    {
                        sw.WriteLine($"Error in method DBApplicationInsights.AddAction():{e.Message} \t {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    }
                    transaction.Rollback();
                }
            }
        }
        public void ClearTable()
        {
            string sqlExpression = "ClearTabel";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Transaction = transaction;

                try
                {
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorFile.txt"), true))
                    {
                        sw.WriteLine($"Error in method DBApplicationInsights.AddAction():{e.Message} \t {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                    }
                    transaction.Rollback();
                }
            }
        }
    }
}