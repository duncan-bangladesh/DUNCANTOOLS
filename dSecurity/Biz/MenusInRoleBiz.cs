using dDataAccess;
using dSecurity.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace dSecurity.Biz
{
    public class MenusInRoleBiz
    {
        private readonly IConfiguration _configuration;
        private string? connectionString = "";
        public MenusInRoleBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<int> AddMenusInRole(MenusInRole menu)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.AddMenusInRole", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RoleId", menu.RoleId);
                    command.Parameters.AddWithValue("@MenuId", menu.MenuId);
                    command.Parameters.AddWithValue("@EntryBy", menu.EntryBy);
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
        public async Task<int> CheckMenusInRole(MenusInRole menu)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.CheckMenusInRole", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RoleId", menu.RoleId);
                    command.Parameters.AddWithValue("@EntryBy", menu.EntryBy);
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
        public async Task<List<MenusInRole>> MenusByRoleId(long RoleId)
        {
            List<MenusInRole> list = new List<MenusInRole>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Security.MenusByRoleId", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@RoleId", RoleId);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MenusInRole model = new MenusInRole();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.RoleId = Convert.ToInt64(reader["RoleId"]);
                        model.MenuId = Convert.ToInt64(reader["MenuId"]);
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
    }
}
