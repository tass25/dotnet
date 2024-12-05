using backend.Models;
using MySql.Data.MySqlClient; // MySQL library
using System.Data;

namespace backend.Repositories
{
    public class OrderRepository : IListRepository<Order>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon"); // MySQL connection string
        }

        public IEnumerable<Order> GetAll()
        {
            string query = @"SELECT OrderID, OrderDateTime, TotalPrice, OrderStatus, UserID FROM `Order`";

            DataTable table = new DataTable();

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                using (MySqlDataReader myReader = myCommand.ExecuteReader())
                {
                    table.Load(myReader);
                }
            }

            var orders = new List<Order>();
            foreach (DataRow row in table.Rows)
            {
                orders.Add(new Order(
                    (int)row["OrderID"],
                    (DateTime)row["OrderDateTime"],
                    (decimal)row["TotalPrice"],
                    Enum.Parse<OrderStatus>(row["OrderStatus"].ToString()),
                    (int)row["UserID"]
                ));
            }
            return orders;
        }

        public List<Order> GetById(int userId)
        {
            string query = @"SELECT OrderID, OrderDateTime, TotalPrice, OrderStatus, UserID FROM `Order` WHERE UserID = @UserID";

            var orders = new List<Order>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order(
                                reader.GetInt32(0),
                                reader.GetDateTime(1),
                                reader.GetDecimal(2),
                                (OrderStatus)reader.GetInt32(3),
                                reader.GetInt32(4)
                            ));
                        }
                    }
                }
            }

            return orders;
        }

        public bool Add(Order order)
{
    // Your original query with CURRENT_TIMESTAMP to auto-set the DateTime
    string query = @"INSERT INTO `Order` (OrderDateTime, TotalPrice, OrderStatus, UserID) 
                     VALUES (CURRENT_TIMESTAMP, @TotalPrice, @Status, @UserID)";

    try
    {
        using (MySqlConnection myCon = new MySqlConnection(_connectionString))
        {
            myCon.Open();
            using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
            {
                // You don't need to set DateTime here, because MySQL handles it with CURRENT_TIMESTAMP
                myCommand.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                myCommand.Parameters.AddWithValue("@Status",(int) order.Status);
                myCommand.Parameters.AddWithValue("@UserID", order.UserID);
                myCommand.ExecuteNonQuery();
            }
        }
        return true;
    }
    catch
    {
        return false;
    }
}


        public bool Update(Order order)
        {
            string query = @"UPDATE `Order` 
                             SET OrderStatus = @OrderStatus,
                                 UserID = @UserID
                             WHERE OrderID = @OrderID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@OrderID", order.OrderID);
                    myCommand.Parameters.AddWithValue("@OrderStatus",(int) order.Status);
                    myCommand.Parameters.AddWithValue("@UserID", order.UserID);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool Delete(int orderId)
        {
            string query = @"DELETE FROM `Order` WHERE OrderID = @OrderID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@OrderID", orderId);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public Order GetObjById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
