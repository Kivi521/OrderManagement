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
{
    /// <summary>
    /// class of orders to manage the orders, connect with all the orders tables of database, contain GetOrderHeaders; GetOrderHeader; InsertOrderHeader; IEnumerable;
    /// UpsertOrderItem; UpdateOrderState; DeleteOrderHeader; Dlete OrderItems
    /// </summary>
    public class Orders
    {
        private string _connectionString;
        /// <summary>
        /// set connetionString a value which is cnnStr
        /// </summary>
        /// <param name="cnnStr"></param>
        public Orders(string cnnStr)
        {
            this._connectionString = cnnStr;
        }
        /// <summary>
        /// GetOrderHeaders method is for getting all the order header from orderheader database table
        /// </summary>
        /// <returns>the list of all the orders from the orderheader database table</returns>
        public IEnumerable<OrderHeader> GetOrderHeaders()
        {
            List<OrderHeader> orders = new List<OrderHeader>();
            string orderHeaderQuery = "Select * from dbo.OrderHeaders";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(this._connectionString))
            using (SqlCommand cmd = new SqlCommand(orderHeaderQuery, cn))
            {

                cn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                // Call Read before accessing data.
                while (reader.Read())
                {
                    OrderHeader header = new OrderHeader();
                    header.Id = int.Parse(reader[0].ToString());
                    header.State = (OrderStates)int.Parse(reader[1].ToString());
                    header.DateTime = Convert.ToDateTime(reader[2]);

                    orders.Add(header);
                }
                // Call Close when done reading.
                reader.Close();
                cn.Close();
            }


            var orderItems = new List<OrderItem>();

            string orderItemQuery = "Select item.*, stock.Name from dbo.OrderItems as item left join dbo.StockItems as stock on item.StockItemId = stock.Id";
            using (SqlConnection cn = new SqlConnection(this._connectionString))
            using (SqlCommand cmd = new SqlCommand(orderItemQuery, cn))
            {

                cn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                // Call Read before accessing data.
                while (reader.Read())
                {
                    OrderItem item = new OrderItem();
                    item.OrderHeaderId = int.Parse(reader[0].ToString());
                    item.StockItemId = int.Parse(reader[1].ToString());
                    item.Description = reader[2].ToString();
                    item.Price = decimal.Parse(reader[3].ToString());
                    item.Quantity = int.Parse(reader[4].ToString());
                    //item.Total = item.Price * item.Quantity;
                    item.Name = reader[5].ToString();
                    orderItems.Add(item);

                    OrderHeader order = orders.First(p => p.Id == item.OrderHeaderId);
                    order.AddOrderItem(item);
                }

                // Call Close when done reading.
                reader.Close();
                cn.Close();
            }


            return orders;
        }
        /// <summary>
        /// GetOrderHeader method is for getting a select order header from all the order headers by id to deal with
        /// </summary>
        /// <param name="id">order header id</param>
        /// <returns>the seleted order header</returns>
        public OrderHeader GetOrderHeader(int id)
        {
            OrderHeader order = GetOrderHeaders().First(header => header.Id == id);

            return order;
        }
        /// <summary>
        ///  InsertOrderHeader method is for insert a new orderheader into orderheader database table
        /// </summary>
        /// <param name="header">the new older </param>
        /// <returns>the insert new order's id</returns>

        public int InsertOrderHeader(OrderHeader header)
        {
            int insertedId = 0;
            string query = "INSERT INTO dbo.OrderHeaders (OrderStateId, DateTime) " +
                   "VALUES (@OrderStateId, @DateTime)  ; SELECT SCOPE_IDENTITY()";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(this._connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@OrderStateId", SqlDbType.Int).Value = (int)header.State;
                cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = header.DateTime;

                // open connection, execute INSERT, close connection
                cn.Open();

                //numOfRows = cmd.ExecuteNonQuery();
                insertedId = Convert.ToInt32(cmd.ExecuteScalar());

                cn.Close();
            }

            return insertedId;
        }

        /// <summary>
        /// GetItemsByHeaderId method is for getting the matched item obey header id
        /// </summary>
        /// <param name="orderHeadId">the selected order header id</param>
        /// <returns> the list of order items</returns>
        public IEnumerable<OrderItem> GetItemsByHeaderId(int orderHeadId)
        {
            var orderItems = new List<OrderItem>();
            string query = "Select * from dbo.OrderItems WHERE  OrderHeaderId = @OrderHeaderId";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(this._connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@OrderHeaderId", SqlDbType.Int).Value = orderHeadId;

                cn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                // Call Read before accessing data.
                while (reader.Read())
                {
                    OrderItem item = new OrderItem();
                    item.OrderHeaderId = int.Parse(reader[0].ToString());
                    item.StockItemId = int.Parse(reader[1].ToString());
                    item.Description = reader[2].ToString();
                    item.Price = decimal.Parse(reader[3].ToString());
                    item.Quantity = int.Parse(reader[4].ToString());
                    orderItems.Add(item);
                }

                // Call Close when done reading.
                reader.Close();
                cn.Close();
            }

            return orderItems;
        }
        /// <summary>
        ///  UpsertOrderItem method has two aims, I will insert an orderItem, if the order item is not exit, the item will be insert, if it is exit, the informatioin will update to the new import.
        /// </summary>
        /// <param name="orderItem">the item that I would like to insert or update</param>
        /// <returns>the number of insert or update orderitem's row </returns>
        public int UpsertOrderItem(OrderItem orderItem)
        {
            int numOfRows = 0;
            string insertQuery = "INSERT INTO dbo.OrderItems (OrderHeaderId, StockItemId, Description, Price, Quantity) " +
                   "VALUES (@OrderHeaderId, @StockItemId, @Description, @Price, @Quantity)";

            string updateQuery = "UPDATE dbo.OrderItems SET Description= @Description, Price = @Price, Quantity = @Quantity" +
                   " WHERE OrderHeaderId = @OrderHeaderId and StockItemId = @StockItemId";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(this._connectionString))
            {
                try
                {
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, cn))
                    {
                        // define parameters and their values
                        insertCmd.Parameters.Add("@OrderHeaderId", SqlDbType.Int).Value = orderItem.OrderHeaderId;
                        insertCmd.Parameters.Add("@StockItemId", SqlDbType.Int).Value = orderItem.StockItemId;

                        insertCmd.Parameters.Add("@Description", SqlDbType.VarChar, 150).Value = orderItem.Description;
                        insertCmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = orderItem.Price;
                        insertCmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = orderItem.Quantity;


                        // open connection, execute INSERT, close connection
                        cn.Open();
                        numOfRows = insertCmd.ExecuteNonQuery();
                        cn.Close();
                    }

                }
                catch
                {
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, cn))
                    {
                        // define parameters and their values
                        updateCmd.Parameters.Add("@OrderHeaderId", SqlDbType.Int).Value = orderItem.OrderHeaderId;
                        updateCmd.Parameters.Add("@StockItemId", SqlDbType.Int).Value = orderItem.StockItemId;

                        updateCmd.Parameters.Add("@Description", SqlDbType.VarChar, 150).Value = orderItem.Description;
                        updateCmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = orderItem.Price;
                        updateCmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = orderItem.Quantity;
                        

                        // open connection, execute INSERT, close connection
                        // cn.Open();
                        numOfRows = updateCmd.ExecuteNonQuery();
                        cn.Close();
                    }
                }
                
            }

            return numOfRows;
        }
        /// <summary>
        /// UpdateOrderState method is for update the order state from new to pending or complete.
        /// </summary>
        /// <param name="order">the selected order</param>
        /// <returns>the number of row that I have update state</returns>
        public int UpdateOrderState(OrderHeader order)
        {
            int UpdateNumOfRows = 0;
            OrderHeader oldOrder = new OrderHeader();
            string UpdateOrderStateQuery = "UPDATE dbo.OrderHeaders SET OrderStateId = @OrderStateId WHERE OrderStateId = @OrderStateId";
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            using (SqlCommand command = new SqlCommand(UpdateOrderStateQuery, connection))
            {
                command.Parameters.Add("@Id", SqlDbType.Int).Value = order.Id;
                command.Parameters.Add("@OrderStateId", SqlDbType.Int).Value = (int)order.State;
                connection.Open();
                UpdateNumOfRows = command.ExecuteNonQuery();
                connection.Close();

            }
                return UpdateNumOfRows;
        }
        /// <summary>
        /// DeleteOrderHeaderAndOrderItems method is for delete the orderheader and delete the order items from the database table
        /// </summary>
        /// <param name="orderHeaderId">the selected orderheader if</param>
        /// <returns>sum of the number of delete order header row and the number of delete order item row</returns>
        public int DeleteOrderHeaderAndOrderItems(int orderHeaderId)
        {
            int deleteOrderHeaderNumOfRows = 0;
            int deleteOrderItemsNumOfRows = 0; 
            OrderHeader Order = new OrderHeader();
            string deleteOrderHeaderQuery = "DELETE dbo.OrderHeaders WHERE Id = @OrderHeaderId";
            string deleteOrderItemsQuery = "DELETE dbo.OrderItems WHERE OrderHeaderId = @OrderHeaderId";

            using (SqlConnection connection = new SqlConnection(this._connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(deleteOrderItemsQuery, connection))
                {
                    command.Parameters.Add("@OrderHeaderId", SqlDbType.Int).Value = orderHeaderId;
                    deleteOrderItemsNumOfRows = command.ExecuteNonQuery();
                }

            
                using (SqlCommand command = new SqlCommand(deleteOrderHeaderQuery, connection))
                {
                    command.Parameters.Add("@OrderHeaderId", SqlDbType.Int).Value = orderHeaderId;
                    deleteOrderHeaderNumOfRows = command.ExecuteNonQuery();
                
                }
                connection.Close();
            }

            return (deleteOrderHeaderNumOfRows + deleteOrderItemsNumOfRows);

        }

    }
    
}
