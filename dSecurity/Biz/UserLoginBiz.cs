using dDataAccess;
using dSecurity.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace dSecurity.Biz
{
    public class UserLoginBiz
    {
        private IConfiguration _configuration;
        private string? connectionString = "";
        public UserLoginBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<int> IsFound(Users user)
        {
            SqlConnection connection = access.GetConnection(connectionString);
            int result = 0;
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.UserLogin", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = Convert.ToInt32(reader["IsFound"]);
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Dispose();
            }
            return await Task.Run(() => result);
        }
        public async Task<int> IsAuthUrl(string userName, string menuUrl)
        {
            SqlConnection connection = access.GetConnection(connectionString);
            int result = 0;
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.IsAuthUrl", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserName", userName);
                    command.Parameters.AddWithValue("@MenuUrl", menuUrl);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = Convert.ToInt32(reader["IsFound"]);
                    }
                    reader.Close();
                    connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Dispose();
            }
            return await Task.Run(() => result);
        }
    }
}
