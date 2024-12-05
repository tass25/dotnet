using backend.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace backend.Repositories
{
    public class ProductSizeRepository : IListRepository<ProductSize>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductSizeRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon"); // Ensure this is for MySQL
        }

        public IEnumerable<ProductSize> GetAll()
        {
            string query = @"SELECT ProductSizeID, Size, Price, Quantity, ProductID FROM PRODUCT_SIZE";

            DataTable table = new DataTable();
            MySqlDataReader myReader;
            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            var productSizes = new List<ProductSize>();
            foreach (DataRow row in table.Rows)
            {
                productSizes.Add(new ProductSize(
                    (int)row["ProductSizeID"],
                    (int)row["Size"],
                    (decimal)row["Price"],
                    (int)row["Quantity"],
                    (int)row["ProductID"]
                ));
            }
            return productSizes;
        }

        public ProductSize GetObjById(int productSizeId)
        {
            string query = @"SELECT * FROM PRODUCT_SIZE WHERE ProductSizeID = @ProductSizeID";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductSizeID", productSizeId);

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        ProductSize productSize = new ProductSize(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetDecimal(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4)
                        );

                        reader.Close();
                        connection.Close();

                        return productSize;
                    }
                    else
                    {
                        reader.Close();
                        connection.Close();

                        return null; 
                    }
                }
            }
        }

        public List<ProductSize> GetById(int productId)
        {
            string query = @"SELECT * FROM PRODUCT_SIZE WHERE ProductID = @ProductID";

            List<ProductSize> productSizes = new List<ProductSize>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);

                    MySqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        ProductSize productSize = new ProductSize(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetDecimal(2),
                            reader.GetInt32(3),
                            reader.GetInt32(4)
                        );

                        productSizes.Add(productSize);
                    }

                    reader.Close();
                }

                connection.Close();
            }

            return productSizes;
        }

        public bool Add(ProductSize productSize)
        {
            string query = @"INSERT INTO PRODUCT_SIZE 
                             (Size, Price, Quantity, ProductID) 
                             VALUES (@Size, @Price, @Quantity, @ProductID)";

            try
            {
                using (MySqlConnection myCon = new MySqlConnection(_connectionString))
                {
                    myCon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Size", productSize.Size);
                        myCommand.Parameters.AddWithValue("@Price", productSize.Price);
                        myCommand.Parameters.AddWithValue("@Quantity", productSize.Quantity);
                        myCommand.Parameters.AddWithValue("@ProductID", productSize.ProductID);
                        myCommand.ExecuteNonQuery();
                        myCon.Close();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ProductSize productSize)
        {
            string query = @"UPDATE PRODUCT_SIZE 
                            SET  Size = @Size, Price = @Price, Quantity = @Quantity
                            WHERE ProductSizeID = @ProductSizeID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductSizeID", productSize.ProductSizeID);
                    myCommand.Parameters.AddWithValue("@Price", productSize.Price);
                    myCommand.Parameters.AddWithValue("@Size", productSize.Size); // Include Size field

                    myCommand.Parameters.AddWithValue("@Quantity", productSize.Quantity);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool Delete(int productSizeID)
        {
            string query = @"DELETE FROM PRODUCT_SIZE WHERE ProductSizeID = @ProductSizeID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductSizeID", productSizeID);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
