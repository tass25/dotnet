using backend.Models;
using MySql.Data.MySqlClient; // MySQL library
using System.Data;

namespace backend.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon");
        }

        public IEnumerable<Product> GetAll()    
        {
            string query = @"SELECT ProductID, Name, Brand, Description, ImageURL FROM `PRODUCT`";

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

            var products = new List<Product>();
            foreach (DataRow row in table.Rows)
            {
                products.Add(new Product(
                    (int)row["ProductID"],
                    row["Name"].ToString(),
                    row["Brand"].ToString(),
                    row["Description"].ToString(),
                    row["ImageURL"].ToString()
                ));
            }
            return products;
        }

        public Product GetById(int productId)
        {
            string query = @"SELECT ProductID, Name, Brand, Description, ImageURL FROM `PRODUCT` WHERE ProductID = @ProductID";

            Product product = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductID", productId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new Product(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetString(3),
                                reader.GetString(4)
                            );
                        }
                    }
                }
            }

            return product;
        }

        public bool Add(Product product)
        {
            string query = @"INSERT INTO `PRODUCT` 
                             (Name, Brand, Description, ImageURL) 
                             VALUES (@Name, @Brand, @Description, @ImageURL)";

            try
            {
                using (MySqlConnection myCon = new MySqlConnection(_connectionString))
                {
                    myCon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Name", product.Name);
                        myCommand.Parameters.AddWithValue("@Brand", product.Brand);
                        myCommand.Parameters.AddWithValue("@Description", product.Description);
                        myCommand.Parameters.AddWithValue("@ImageURL", product.ImageURL);
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

        public bool Update(Product product)
        {
            string query = @"UPDATE `PRODUCT` 
                             SET Name = @Name,
                                 Brand = @Brand,
                                 Description = @Description,
                                 ImageURL = @ImageURL
                             WHERE ProductID = @ProductID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductID", product.ProductID);
                    myCommand.Parameters.AddWithValue("@Name", product.Name);
                    myCommand.Parameters.AddWithValue("@Brand", product.Brand);
                    myCommand.Parameters.AddWithValue("@Description", product.Description);
                    myCommand.Parameters.AddWithValue("@ImageURL", product.ImageURL);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0; 
                }
            }
        }

        public bool Delete(int productId)
        {
            string query = @"DELETE FROM `PRODUCT` 
                             WHERE ProductID = @ProductID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@ProductID", productId);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
