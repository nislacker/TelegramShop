using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace TelegramShop
{
    class Program
    {
        static void Main(string[] args)
        {
            #region DatabaseOperations

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            // "Database=eshop;Datasource=localhost;User=root";

            try
            {
                using (MySqlConnection sqlConnection = new MySqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;

                    sqlConnection.Open();

                    MySqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT * FROM category;";
                    sqlCommand.Connection = sqlConnection;

                    // Подсчитать кол-во строк в таблице
                    //object result = (new MySqlCommand("SELECT COUNT(*) FROM category", sqlConnection).ExecuteScalar());
                    //Console.WriteLine(((Int64)result));


                    using (DbDataReader reader = sqlCommand.ExecuteReader())
                    {
                        // вывести названия полей (столбцов) таблицы
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine((reader).GetName(i));
                        }
                        Console.WriteLine();

                        Console.WriteLine("Field Count = " + reader.FieldCount);

                        Console.WriteLine($"Id:\tName:\tSortOrder:\tStatus:");

                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[0]}\t{reader["name"]}\t{reader["sort_order"]}\t{reader["status"]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            #endregion DatabaseOperations
        }
    }
}
