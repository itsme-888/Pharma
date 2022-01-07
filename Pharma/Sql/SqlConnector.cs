using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Pharma.Models;

namespace Pharma.Sql
{
    public static class SqlConnector
    {
        private readonly static string connectionString = ConfigurationManager.ConnectionStrings["PharmaDb"].ConnectionString;

        public static int Insert(BaseEntity entity)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(QueryBuilder.GenerateAddScript(entity), connection);
                return command.ExecuteNonQuery();
            }
        }

        public static int Count(Type type)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(QueryBuilder.GenerateCountScript(type), connection);
                return (int)command.ExecuteScalar();
            }
        }

        public static List<string[]> SelectFromProductRemainingView(int pharmaId)
        {
            List<string[]> result = new List<string[]>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(QueryBuilder.GenerateSelectFromProductRemainingView(pharmaId), connection);
                var reader = command.ExecuteReader();

                string[] headers = { "Товар", "Количество товара" };
                result.Add(headers);

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string[] values = new string[2];
                        values[0] = reader[headers[0]].ToString();
                        values[1] = reader[headers[1]].ToString();
                        result.Add(values);
                    }
                }
                reader.Close();
            }
            return result;
        }


        public static List<string[]> Select(Type type)
        {
            List<string[]> result = new List<string[]>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(QueryBuilder.GenerateSelectScript(type), connection);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    var propList = type.GetProperties().Select(prop => prop.Name).ToArray();
                    result.Add(propList);

                    while (reader.Read())
                    {
                        string[] values = new string[propList.Length];

                        for(int i = 0; i < propList.Length; i++)
                        {
                            values[i] = reader[propList[i]].ToString();
                        }

                        result.Add(values);
                    }
                }
                reader.Close();
            }
            return result;
        }

        public static int Delete(Type type, int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(QueryBuilder.GenerateDeleteScript(type, id), connection);
                return command.ExecuteNonQuery();
            }
        }
    }
}
