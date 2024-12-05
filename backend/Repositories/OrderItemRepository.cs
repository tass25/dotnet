using backend.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace backend.Repositories
{
    public class OrderItemRepository : IListRepository<OrderItem>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public OrderItemRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon");
        }

        public IEnumerable<OrderItem> GetAll()
        {
            string query = @"SELECT OrderItemID, Quantity, OrderID, ProductSizeID FROM `ORDER_ITEM`";

            DataTable table = new DataTable();
            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    using (MySqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }
                }
            }

            var orderItems = new List<OrderItem>();
            foreach (DataRow row in table.Rows)
            {
                orderItems.Add(new OrderItem(
                    (int)row["OrderItemID"],
                    (int)row["Quantity"],
                    (int)row["OrderID"],
                    (int)row["ProductSizeID"]
                ));
            }
            return orderItems;
        }

        public List<OrderItem> GetById(int orderId)
        {
            string query = @"SELECT * FROM `ORDER_ITEM` WHERE OrderID = @OrderID";

            List<OrderItem> orderItems = new List<OrderItem>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderID", orderId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OrderItem orderItem = new OrderItem(
                                reader.GetInt32(0),
                                reader.GetInt32(1),
                                reader.GetInt32(2),
                                reader.GetInt32(3)
                            );

                            orderItems.Add(orderItem);
                        }
                    }
                }
            }

            return orderItems;
        }

        public bool Add(OrderItem orderItem)
        {
            string query = @"INSERT INTO `ORDER_ITEM` 
                             (Quantity, OrderID, ProductSizeID) 
                             VALUES (@Quantity, @OrderID, @ProductSizeID)";

            try
            {
                using (MySqlConnection myCon = new MySqlConnection(_connectionString))
                {
                    myCon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
                        myCommand.Parameters.AddWithValue("@OrderID", orderItem.OrderID);
                        myCommand.Parameters.AddWithValue("@ProductSizeID", orderItem.ProductSizeID);
                        myCommand.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(OrderItem orderItem)
        {
            string query = @"UPDATE `ORDER_ITEM`
                             SET Quantity = @Quantity,
                                 OrderID = @OrderID,
                                 ProductSizeID = @ProductSizeID
                             WHERE OrderItemID = @OrderItemID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@OrderItemID", orderItem.OrderItemID);
                    myCommand.Parameters.AddWithValue("@Quantity", orderItem.Quantity);
                    myCommand.Parameters.AddWithValue("@OrderID", orderItem.OrderID);
                    myCommand.Parameters.AddWithValue("@ProductSizeID", orderItem.ProductSizeID);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool Delete(int orderItemId)
        {
            string query = @"DELETE FROM `ORDER_ITEM` WHERE OrderItemID = @OrderItemID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@OrderItemID", orderItemId);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public OrderItem GetObjById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
