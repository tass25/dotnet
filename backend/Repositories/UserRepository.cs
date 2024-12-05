using backend.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace backend.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("UserAppCon"); // Ensure this is for MySQL
        }

        public IEnumerable<User> GetAll()
        {
            string query = @"SELECT UserID, FirstName, LastName, Email, Phone, Password, Address, City, PostalCode FROM USER";

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

            var users = new List<User>();
            foreach (DataRow row in table.Rows)
            {
                users.Add(new User(
                    (int)row["UserID"],
                    row["FirstName"].ToString(),
                    row["LastName"].ToString(),
                    row["Email"].ToString(),
                    row["Phone"].ToString(),
                    row["Password"].ToString(),
                    row["Address"] != DBNull.Value ? row["Address"].ToString() : null,
                    row["City"] != DBNull.Value ? row["City"].ToString() : null,
                    row["PostalCode"] != DBNull.Value ? row["PostalCode"].ToString() : null
                ));
            }
            return users;
        }

        public User GetById(int userId)
        {
            string query = @"SELECT UserID, FirstName, LastName, Email, Phone, Password, Address, City, PostalCode FROM USER WHERE UserID = @UserID";

            User user = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);

                    MySqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        user = new User(
                         reader.GetInt32(0),
                         reader.GetString(1),
                         reader.GetString(2),
                         reader.GetString(3),
                         reader.GetString(4),
                         reader.GetString(5),
                         reader["Address"] != DBNull.Value ? reader.GetString(6) : null,
                         reader["City"] != DBNull.Value ? reader.GetString(7) : null,
                         reader["PostalCode"] != DBNull.Value ? reader.GetString(8) : null
                     );
                    }

                    reader.Close();
                }

                connection.Close();
            }

            return user;
        }

        public bool Add(User user)
        {
            string query = @"INSERT INTO USER 
                             (FirstName, LastName, Email, Phone, Password, Address, City, PostalCode) 
                             VALUES (@FirstName, @LastName, @Email, @Phone, @Password, @Address, @City, @PostalCode)";

            try
            {
                using (MySqlConnection myCon = new MySqlConnection(_connectionString))
                {
                    myCon.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                        myCommand.Parameters.AddWithValue("@LastName", user.LastName);
                        myCommand.Parameters.AddWithValue("@Email", user.Email);
                        myCommand.Parameters.AddWithValue("@Phone", user.Phone);
                        myCommand.Parameters.AddWithValue("@Password", user.Password);
                        myCommand.Parameters.AddWithValue("@Address", user.Address);
                        myCommand.Parameters.AddWithValue("@City", user.City);
                        myCommand.Parameters.AddWithValue("@PostalCode", user.PostalCode);

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

        public bool Update(User user)
        {
            string query = @"UPDATE USER 
                             SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone, Password = @Password, 
                                 Address = @Address, City = @City, PostalCode = @PostalCode 
                             WHERE UserID = @UserID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserID", user.UserID);
                    myCommand.Parameters.AddWithValue("@FirstName", user.FirstName);
                    myCommand.Parameters.AddWithValue("@LastName", user.LastName);
                    myCommand.Parameters.AddWithValue("@Email", user.Email);
                    myCommand.Parameters.AddWithValue("@Phone", user.Phone);
                    myCommand.Parameters.AddWithValue("@Password", user.Password);
                    myCommand.Parameters.AddWithValue("@Address", user.Address);
                    myCommand.Parameters.AddWithValue("@City", user.City);
                    myCommand.Parameters.AddWithValue("@PostalCode", user.PostalCode);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        public bool Delete(int userId)
        {
            string query = @"DELETE FROM USER WHERE UserID = @UserID";

            using (MySqlConnection myCon = new MySqlConnection(_connectionString))
            {
                myCon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserID", userId);

                    int rowsAffected = myCommand.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
