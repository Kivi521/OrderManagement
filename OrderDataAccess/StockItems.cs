using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderDomain;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace OrderDataAccess
{/// <summary>
/// StockItems method is for manage stock Items, connect with stockItems table of database
/// </summary>
    public class StockItems
    {
        private string _connectionString;
        public StockItems(string connString)
        {
            this._connectionString = connString;
        }
        /// <summary>
        /// GetStockItems method if for getting all the stockItems of [add order item view] from stockItems table of database
        /// </summary>
        /// <returns>all the stock items and it's price</returns>
        public IEnumerable<StockItem> GetStockItems()
        {
            var stockItem = new List<StockItem>();
           

            string queryString = "SELECT * FROM dbo.StockItems;";

            using (SqlConnection connection =  new SqlConnection(this._connectionString))
            using (SqlCommand command = new SqlCommand(queryString, connection)) 
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                // Call Read before accessing data.
                while (reader.Read())
                {
                    StockItem SI = new StockItem();
                    SI.Id = int.Parse(reader[0].ToString());
                    SI.InStock = int.Parse(reader[3].ToString());
                    SI.Name = reader[1].ToString();
                    SI.Price = decimal.Parse(reader[2].ToString());
                    stockItem.Add(SI);

                }
                
                // Call Close when done reading.
                reader.Close();
                connection.Close();
            }
            return stockItem;
        }

        /// <summary>
        /// GetStockItem method if for getting one stock Item by item id stockItems table of database
        /// </summary>
        /// <param name="id">item id</param>
        /// <returns>the collected stock item</returns>
        public StockItem GetStockItem(int id)
        {
            StockItem item = new StockItem();
            string queryString = "SELECT Id, Name, Price, InStock  From dbo.StockItems WHERE Id = @id" ;
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            using (SqlCommand command = new SqlCommand(queryString, connection))
            {
                command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                item.Id = int.Parse(reader[0].ToString());
                item.Name = reader[1].ToString();
                item.Price = decimal.Parse(reader[2].ToString());
                item.InStock = int.Parse(reader[3].ToString());
                reader.Close();
                connection.Close();
            }
            return item;
        }
        /// <summary>
        /// UpdateStockItemAmount method is for update the stockItem Amount, if the customer would like to change their amount of collecte item, they could use this method
        /// </summary>
        /// <param name="order">an orderHeader</param>
        /// <returns>the number of rows that the Item amount be changed </returns>
        public int UpdateStockItemAmount(OrderHeader order)
        {
            int updateNumOfRows = 0;

            foreach(OrderItem item in order._orderItems)
            {
                // item.StockItemId, item.Quantity
                string UpdateStockItemAmountQuery = "Update dbo.StockItems SET InStock = InStock - @Quantity WHERE Id = @StockItemId";
                using (SqlConnection connection = new SqlConnection(this._connectionString))
                using (SqlCommand command = new SqlCommand(UpdateStockItemAmountQuery, connection))
                {
                    // define parameters and their values
                    command.Parameters.Add("@StockItemId", SqlDbType.Int).Value = item.StockItemId;
                    command.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;

                    connection.Open();
                    //change the amount of older
                    updateNumOfRows += command.ExecuteNonQuery();
                    connection.Close();
                }


            }
            
             return updateNumOfRows;
        }



    }
}
