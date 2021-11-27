using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using OrderDomain;

namespace OrderSystemUI.Data

{
    
    public class ManageData
    {
        private string _connectionString;
        public ManageData()
        {
            this._connectionString =
                System.Configuration.ConfigurationManager.
                    ConnectionStrings["OrderManagementDb"].ConnectionString;
        }
        public IEnumerable<StockItem> GetStockItems()
        {
            var stockItem = new List<StockItem>();
            //TODO: (Task 2A) 
            //add the logic for retrieving all People records from the database and populatinng the list with the retrieved data so it can be returned

            string queryString = "SELECT * FROM dbo.StockItems;";

            using (SqlConnection connection =
                       new SqlConnection(this._connectionString))
            {
                SqlCommand command =
                    new SqlCommand(queryString, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                // Call Read before accessing data.
                while (reader.Read())
                {
                    StockItem SI = new StockItem();
                    SI.Id = int.Parse(reader[0].ToString());
                    SI.InStock = int.Parse(reader[3].ToString());
                    SI.Name = reader[1].ToString();
                    SI.Price = double.Parse(reader[2].ToString());
                    stockItem.Add(SI);
                }

                // Call Close when done reading.
                reader.Close();
            }
            return stockItem;
        }
    }
}
