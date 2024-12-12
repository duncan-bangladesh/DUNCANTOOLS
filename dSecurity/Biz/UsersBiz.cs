using dDataAccess;
using dSecurity.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;

namespace dSecurity.Biz
{
    public class UsersBiz
    {
        private readonly IConfiguration _configuration;
        private string? connectionString = "";
        public UsersBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<Users>> GetUsers()
        {
            List<Users> users = new List<Users>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Security.GetAllUsers", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Users user = new Users();
                        user.UserId = Convert.ToInt64(reader["UserId"]);
                        user.FullName = reader["FullName"].ToString();
                        user.EmailAddress = reader["EmailAddress"].ToString();
                        user.MobileNumber = reader["MobileNumber"].ToString();
                        user.CompanyId = Convert.ToInt64(reader["CompanyId"]);
                        user.CompanyName = reader["CompanyName"].ToString();
                        user.UserName = reader["UserName"].ToString();

                        //user.Password = reader["Password"].ToString();
                        //user.PasswordHash = reader["PasswordHash"].ToString();

                        user.Password = "";
                        user.PasswordHash = "";
                        user.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        user.EntryBy = reader["EntryBy"].ToString();
                        user.EntryDate = reader["EntryDate"].ToString();
                        user.ModifyBy = reader["ModifyBy"].ToString();
                        user.ModifyDate = reader["ModifyDate"].ToString();
                        users.Add(user);
                    }
                }
                connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                connection.Dispose();
            }
            return await Task.Run(() => users);
        }
        public async Task<int> AddUser(Users user)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.AddUser", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                    command.Parameters.AddWithValue("@MobileNumber", user.MobileNumber);
                    command.Parameters.AddWithValue("@CompanyId", user.CompanyId);
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@EntryBy", user.EntryBy);
                    result = command.ExecuteNonQuery();
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
        public async Task<int> UpdateUserInfo(Users user)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.UpdateUserInfo", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@EmailAddress", user.EmailAddress);
                    command.Parameters.AddWithValue("@MobileNumber", user.MobileNumber);
                    command.Parameters.AddWithValue("@CompanyId", user.CompanyId);
                    command.Parameters.AddWithValue("@ModifyBy", user.ModifyBy);
                    result = command.ExecuteNonQuery();
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
        public async Task<int> CheckUser(string UserName)
        {
            SqlConnection connection = access.GetConnection(connectionString);
            int result = 0;
            try
            {
                if (!string.IsNullOrEmpty(UserName))
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand command = new SqlCommand("Security.CheckUserName", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@UserName", UserName);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result = Convert.ToInt32(reader["IsFound"]);
                        }
                        reader.Close();
                        connection.Close();
                    }
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
        public async Task<FraLoginInfo> FraInfoForLoginByUserName(string? UserName)
        {
            SqlConnection connection = access.GetConnection(connectionString);
            var result = new FraLoginInfo();
            try
            {
                if (!string.IsNullOrEmpty(UserName))
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand command = new SqlCommand("Security.FraInfoForLoginByUserName", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@UserName", UserName);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            FraLoginInfo info = new FraLoginInfo();
                            info.UserFullName = reader["FullName"].ToString();
                            info.LoginCompanyId =Convert.ToInt64(reader["CompanyId"]);
                            info.FraCompanyCode = reader["FraCompanyCode"].ToString();
                            info.FraCompanyName = reader["FraCompanyName"].ToString();
                            info.FraDivisionCode = reader["FraDivisionCode"].ToString();
                            info.FraDivisionName = reader["FraDivisionName"].ToString();
                            result = info;
                        }
                        reader.Close();
                        connection.Close();
                    }
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
        public async Task<int> CheckPassword(string? UserName, string? Password)
        {
            SqlConnection connection = access.GetConnection(connectionString);
            int result = 0;
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.CheckPassword", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserName", UserName);
                    command.Parameters.AddWithValue("@Password", Password);
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
        public async Task<int> ChangeUserStatus(Users user)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.ChangeUserStatus", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserId", user.UserId);
                    command.Parameters.AddWithValue("@IsActive", user.IsActive);
                    command.Parameters.AddWithValue("@ModifyBy", user.ModifyBy);
                    result = command.ExecuteNonQuery();
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
            return await Task.Run(()=> result);
        }
        public async Task<int> ChangePassword(Users user)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.ChangePassword", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserName", user.UserName);
                    command.Parameters.AddWithValue("@OldPassword", user.OldPassword);
                    command.Parameters.AddWithValue("@NewPassword", user.Password);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@EntryBy", user.EntryBy);
                    result = command.ExecuteNonQuery();
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
        public async Task<long> CompanyIdByUserName(string UserName)
        {
            SqlConnection connection = access.GetConnection(connectionString);
            long result = 0;
            try
            {
                if (!string.IsNullOrEmpty(UserName))
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        SqlCommand command = new SqlCommand("Security.GetCompanyByUserId", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@UserName", UserName);
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            result = Convert.ToInt64(reader["CompanyId"]);
                        }
                        reader.Close();
                        connection.Close();
                    }
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
            return await Task.Run(()=> result);
        }
        public async Task<string> ShaEncrypt(string? text)
        {
            using (var sha = new SHA1Managed())
            {
                var shaHash = sha.ComputeHash(Encoding.UTF8.GetBytes(text ?? ""));
                var sb = new StringBuilder(shaHash.Length * 2);
                foreach (byte b in shaHash)
                {
                    sb.Append(b.ToString("X2").ToLower());
                }
                var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                return await Task.Run(() => System.Convert.ToBase64String(bytes));
            }            
        }
        public async Task<string> Encrypts(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return await Task.Run(() => System.Convert.ToBase64String(bytes));
        }
        public async Task<string> Decrypts(string text)
        {
            var bytes = System.Convert.FromBase64String(text);
            return await Task.Run(() => System.Text.Encoding.UTF8.GetString(bytes));
        }
    }
}
