using dDataAccess;
using dSecurity.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace dSecurity.Biz
{
    public class MenuBiz
    {
        private readonly IConfiguration _configuration;
        private string? connectionString = "";
        public MenuBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<Menu>> GetMenu()
        {
            List<Menu> list = new List<Menu>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Security.GetAllMenu", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Menu model = new Menu();
                        model.MenuId = Convert.ToInt64(reader["MenuId"]);
                        model.DisplayName = reader["DisplayName"].ToString();
                        model.ControllerName = reader["ControllerName"].ToString();
                        model.ActionName = reader["ActionName"].ToString();
                        model.MenuUrl = reader["MenuUrl"].ToString();
                        model.IsParentMenu = Convert.ToInt32(reader["IsParentMenu"]);
                        model.ParentMenuId = Convert.ToInt32(reader["ParentMenuId"]);
                        model.IconTag = reader["IconTag"].ToString();
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
        public async Task<List<Menu>> GetParentMenu()
        {
            List<Menu> list = new List<Menu>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Security.GetParentMenu", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Menu model = new Menu();
                        model.MenuId = Convert.ToInt64(reader["MenuId"]);
                        model.DisplayName = reader["DisplayName"].ToString();
                        model.ControllerName = reader["ControllerName"].ToString();
                        model.ActionName = reader["ActionName"].ToString();
                        model.MenuUrl = reader["MenuUrl"].ToString();
                        model.IsParentMenu = Convert.ToInt32(reader["IsParentMenu"]);
                        model.ParentMenuId = Convert.ToInt32(reader["ParentMenuId"]);
                        model.IconTag = reader["IconTag"].ToString();
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
        public async Task<int> AddMenu(Menu menu)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.AddMenu", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@DisplayName", menu.DisplayName);
                    command.Parameters.AddWithValue("@ControllerName", menu.ControllerName);
                    command.Parameters.AddWithValue("@ActionName", menu.ActionName);
                    command.Parameters.AddWithValue("@MenuUrl", menu.MenuUrl);
                    command.Parameters.AddWithValue("@IsParentMenu", menu.IsParentMenu);
                    command.Parameters.AddWithValue("@ParentMenuId", menu.ParentMenuId);
                    command.Parameters.AddWithValue("@IconTag", menu.IconTag);
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
        public async Task<int> CheckMenuDisplayName(string name)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.CheckMenuDisplayName", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@DisplayName", name);
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
        public async Task<int> UpdateMenus(Menu menu)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.UpdateMenus", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@MenuId", menu.MenuId);
                    command.Parameters.AddWithValue("@DisplayName", menu.DisplayName);
                    command.Parameters.AddWithValue("@ControllerName", menu.ControllerName);
                    command.Parameters.AddWithValue("@ActionName", menu.ActionName);
                    command.Parameters.AddWithValue("@MenuUrl", menu.MenuUrl);
                    command.Parameters.AddWithValue("@IsParentMenu", menu.IsParentMenu);
                    command.Parameters.AddWithValue("@ParentMenuId", menu.ParentMenuId);
                    command.Parameters.AddWithValue("@IconTag", menu.IconTag);
                    command.Parameters.AddWithValue("@ModifyBy", menu.ModifyBy);
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
        public async Task<int> ChangeMenuStatus(Menu menu)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Security.ChangeMenuStatus", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@MenuId", menu.MenuId);
                    command.Parameters.AddWithValue("@IsActive", menu.IsActive);
                    command.Parameters.AddWithValue("@ModifyBy", menu.ModifyBy);
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
        public async Task<List<Menu>> GetMenusByUser(string? UserName)
        {
            List<Menu> list = new List<Menu>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Security.GetMenusByUser", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@UserName", UserName);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Menu model = new Menu();
                        model.MenuId = Convert.ToInt64(reader["MenuId"]);
                        model.DisplayName = reader["DisplayName"].ToString();
                        model.ControllerName = reader["ControllerName"].ToString();
                        model.ActionName = reader["ActionName"].ToString();
                        model.MenuUrl = reader["MenuUrl"].ToString();
                        model.IsParentMenu = Convert.ToInt32(reader["IsParentMenu"]);
                        model.ParentMenuId = Convert.ToInt32(reader["ParentMenuId"]);
                        model.IconTag = reader["IconTag"].ToString();
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
    }
}
