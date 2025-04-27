using dDataAccess;
using dShared.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace dShared.Biz
{
    public class CompanyBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public CompanyBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<Company>> GetCompanies()
        {
            List<Company> list = new List<Company>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Shared.AllCompanies", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Company model = new Company();
                        model.CompanyId = Convert.ToInt64(reader["CompanyId"]);
                        model.CompanyName = reader["CompanyName"].ToString();
                        model.CompanyCode = reader["CompanyCode"].ToString()!;
                        model.ShortCode = reader["ShortCode"].ToString()!;
                        model.GardenId = Convert.ToInt32(reader["GardenId"]);
                        model.IsTeaEstate = Convert.ToInt32(reader["IsTeaEstate"]);
                        model.IsTranCompany = Convert.ToInt32(reader["IsTranCompany"]);
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
        public async Task<int> AddCompany(Company model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Shared.AddCompany", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CompanyName", model.CompanyName);
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
        public async Task<int> UpdateCompany(Company model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Shared.UpdateCompany", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                    command.Parameters.AddWithValue("@CompanyName", model.CompanyName);
                    command.Parameters.AddWithValue("@ModifyBy", model.ModifyBy);
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
        public async Task<int> CheckCompanyName(string name)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Shared.CheckCompanyName", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CompanyName", name);
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
        //public int UpdateRoles(Role role)
        //{
        //    string? connectionString = Configuration.GetConnectionString("DefaultConnection");
        //    int result = 0;
        //    SqlConnection connection = access.GetConnection(connectionString);
        //    try
        //    {
        //        if (connection.State == System.Data.ConnectionState.Open)
        //        {
        //            SqlCommand command = new SqlCommand("Security.UpdateRoles", connection);
        //            command.CommandType = System.Data.CommandType.StoredProcedure;
        //            command.Parameters.Clear();
        //            command.Parameters.AddWithValue("@UserId", role.RoleId);
        //            command.Parameters.AddWithValue("@RoleName", role.RoleName);
        //            command.Parameters.AddWithValue("@IsActive", role.IsActive);
        //            command.Parameters.AddWithValue("@ModifyBy", role.ModifyBy);
        //            result = command.ExecuteNonQuery();
        //            connection.Close();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        connection.Dispose();
        //    }
        //    return result;
        //}
        public async Task<int> ChangeCompanyStatus(Company model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Shared.ChangeCompanyStatus", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CompanyId", model.CompanyId);
                    command.Parameters.AddWithValue("@IsActive", model.IsActive);
                    command.Parameters.AddWithValue("@ModifyBy", model.ModifyBy);
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
