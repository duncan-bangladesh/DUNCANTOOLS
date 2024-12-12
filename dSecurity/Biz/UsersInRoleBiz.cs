using dDataAccess;
using dSecurity.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace dSecurity.Biz
{
    public class UsersInRoleBiz
    {
        private readonly IConfiguration _configuration;
        private string? connectionString;
        public UsersInRoleBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<int> AddUsersInRole(UsersInRole model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.AddUsersInRole", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RoleId", model.RoleId);
                    command.Parameters.AddWithValue("@UserId", model.UserId);
                    command.Parameters.AddWithValue("@EntryBy", model.EntryBy);
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
        public async Task<int> CheckUsersInRole(UsersInRole model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.CheckUsersInRole", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RoleId", model.RoleId);
                    command.Parameters.AddWithValue("@EntryBy", model.EntryBy);
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
        public async Task<List<UsersInRole>> UsersByRoleId(long RoleId)
        {
            List<UsersInRole> list = new List<UsersInRole>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Security.UsersByRoleId", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@RoleId", RoleId);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        UsersInRole model = new UsersInRole();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.RoleId = Convert.ToInt64(reader["RoleId"]);
                        model.UserId = Convert.ToInt64(reader["UserId"]);
                        model.ApprovalStatus = reader["ApprovalStatus"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.EntryBy = reader["EntryBy"].ToString();
                        model.EntryDate = reader["EntryDate"].ToString();
                        model.ModifyBy = reader["ApprovedBy"].ToString();
                        model.ModifyDate = reader["ApprovedDate"].ToString();
                        list.Add(model);
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
            return await Task.Run(() => list);
        }
        public async Task<int> GetRoleByUser(string UserName)
        {
            SqlConnection connection = access.GetConnection(connectionString);
            int result = 0;
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.GetRoleByUser", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserName", UserName);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = Convert.ToInt32(reader["RoleId"]);
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
            return await Task.Run(()=> result);
        }
    }
}
