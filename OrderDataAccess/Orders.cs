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
    public class Orders
    {
        private string _connectionString;
        public Orders(string cnnStr)
        {
            this._connectionString = cnnStr;
        }

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

            string orderItemQuery = "Select * from dbo.OrderItems";
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

        public OrderHeader GetOrderHeader(int id)
        {
            OrderHeader order = GetOrderHeaders().First(header => header.Id == id);

            return order;
        }

        public int InsertOrderHeader(OrderHeader header)
        {
            int numOfRows = 0;
            string query = "INSERT INTO dbo.OrderHeaders (OrderStateId, DateTime) " +
                   "VALUES (@OrderStateId, @DateTime) ";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(this._connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                // define parameters and their values
                cmd.Parameters.Add("@OrderStateId", SqlDbType.Int).Value = (int)header.State;
                cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = header.DateTime;

                // open connection, execute INSERT, close connection
                cn.Open();
                numOfRows = cmd.ExecuteNonQuery();
                cn.Close();
            }

            return numOfRows;
        }


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

        public int DeleteOrderItem(int orderHeaderId, int stockItemId)
        {
            int deleteOrderItemsNumOfRows = 0;
            string DeleteOrderItemQuery = "DELETE dbo.OrderItems WHERE OrderHeaderId = @OrderHeaderId and StockItemId = @StockItemId";
            using (SqlConnection connection = new SqlConnection(this._connectionString))
            using (SqlCommand command = new SqlCommand(DeleteOrderItemQuery, connection))
            {
                command.Parameters.Add("@OrderHeaderId", SqlDbType.Int).Value = orderHeaderId;
                command.Parameters.Add("@StockItemId", SqlDbType.Int).Value = stockItemId;
                connection.Open();
                deleteOrderItemsNumOfRows = command.ExecuteNonQuery();
                connection.Close();
            }           
             return deleteOrderItemsNumOfRows;
        }
    }
    
}
