using dDataAccess;
using Procurement.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Procurement.Biz
{
    public class SupplierProfileBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public SupplierProfileBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DBProcConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<string> NewSupplierCode(string SupplierName)
        {
            string result = "";
            SqlConnection connection = access.GetConnection(connectionString);
            SqlDataReader? reader = null;
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("sp_GetSupplierCodeBySupplierName", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@SupplierName", SupplierName);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = reader["SupplierCode"].ToString()!;
                    }
                    connection.Close();
                }
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
            return await Task.Run(() => result);
        }
        public async Task<int> CheckDuplicateSupplierName(string SupplierName)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            SqlDataReader? reader = null;
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("sp_CheckDuplicateSupplierName", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@SupplierName", SupplierName);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = Convert.ToInt32(reader["NoOfRecord"]);
                    }
                    connection.Close();
                }
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
            return await Task.Run(() => result);
        }
        public async Task<int> SaveSupplierProfile(SupplierProfile model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("sp_SaveSupplierProfile", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Code", model.Code);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@Address", model.Address);
                    command.Parameters.AddWithValue("@City", model.City);
                    command.Parameters.AddWithValue("@Country", model.Country);
                    command.Parameters.AddWithValue("@Bank", model.Bank);
                    command.Parameters.AddWithValue("@AccountNo", model.AccountNo);
                    command.Parameters.AddWithValue("@RoutingNo", model.RoutingNo);
                    command.Parameters.AddWithValue("@Taxgroup", model.Taxgroup);
                    command.Parameters.AddWithValue("@TIN", model.TIN);
                    command.Parameters.AddWithValue("@BIN", model.BIN);
                    command.Parameters.AddWithValue("@Phone", model.Phone);
                    command.Parameters.AddWithValue("@email", model.email);
                    command.Parameters.AddWithValue("@CreateUser", model.CreateUser);
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
        public async Task<int> UpdateSupplierProfile(SupplierProfile model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("sp_UpdateSupplierProfile", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@SLNo", model.SLNo);
                    command.Parameters.AddWithValue("@Description", model.Description);
                    command.Parameters.AddWithValue("@Address", model.Address);
                    command.Parameters.AddWithValue("@City", model.City);
                    command.Parameters.AddWithValue("@Country", model.Country);
                    command.Parameters.AddWithValue("@Bank", model.Bank);
                    command.Parameters.AddWithValue("@AccountNo", model.AccountNo);
                    command.Parameters.AddWithValue("@RoutingNo", model.RoutingNo);
                    command.Parameters.AddWithValue("@Taxgroup", model.Taxgroup);
                    command.Parameters.AddWithValue("@TIN", model.TIN);
                    command.Parameters.AddWithValue("@BIN", model.BIN);
                    command.Parameters.AddWithValue("@Phone", model.Phone);
                    command.Parameters.AddWithValue("@email", model.email);
                    command.Parameters.AddWithValue("@UpdateUser", model.UpdateUser);
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
        public async Task<List<SupplierProfile>> GetSupplierProfiles()
        {
            List<SupplierProfile> list = new List<SupplierProfile>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("sp_GetAllSupplierProfile", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SupplierProfile model = new SupplierProfile();
                        model.SLNo = Convert.ToInt32(reader["SLNo"]);
                        model.Code = reader["Code"].ToString();
                        model.Description = reader["Description"].ToString();
                        model.Address = reader["Address"].ToString();
                        model.City = reader["City"].ToString();
                        model.Country = reader["Country"].ToString();
                        model.Bank = reader["Bank"].ToString();
                        model.AccountNo = reader["AccountNo"].ToString();
                        model.RoutingNo = reader["RoutingNo"].ToString();
                        model.Taxgroup = Convert.ToInt32(reader["Taxgroup"]);
                        model.TIN = reader["TIN"].ToString();
                        model.BIN = reader["BIN"].ToString();
                        model.Phone = reader["Phone"].ToString();
                        model.email = reader["email"].ToString();
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
