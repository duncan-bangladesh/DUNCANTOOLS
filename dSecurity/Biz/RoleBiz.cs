using dDataAccess;
using dSecurity.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace dSecurity.Biz
{
    public class RoleBiz
    {
        private readonly IConfiguration _configuration;
        private string? connectionString = "";
        public RoleBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<Role>> GetRole()
        {
            List<Role> list = new List<Role>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Security.GetAllRoles", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Role model = new Role();
                        model.RoleId = Convert.ToInt64(reader["RoleId"]);
                        model.RoleName = reader["RoleName"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.EntryBy = reader["EntryBy"].ToString();
                        model.EntryDate = reader["EntryDate"].ToString();
                        model.ModifyBy = reader["ModifyBy"].ToString();
                        model.ModifyDate = reader["ModifyDate"].ToString();
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
        public async Task<int> AddRole(Role role)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.AddRole", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RoleName", role.RoleName);
                    command.Parameters.AddWithValue("@EntryBy", role.EntryBy);
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
        public async Task<int> CheckRole(string name)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.CheckRoleName", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RoleName", name);
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
        public async Task<int> UpdateRoles(Role role)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.UpdateRoles", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RoleId", role.RoleId);
                    command.Parameters.AddWithValue("@RoleName", role.RoleName);
                    command.Parameters.AddWithValue("@ModifyBy", role.ModifyBy);
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
        public async Task<int> ChangeRoleStatus(Role role)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.ChangeRoleStatus", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RoleId", role.RoleId);
                    command.Parameters.AddWithValue("@IsActive", role.IsActive);
                    command.Parameters.AddWithValue("@ModifyBy", role.ModifyBy);
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
    }
}
