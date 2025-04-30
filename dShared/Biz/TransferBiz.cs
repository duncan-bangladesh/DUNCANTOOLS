using dDataAccess;
using dShared.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dShared.Biz
{

    public class TransferBiz
    {
        private readonly IConfiguration _configuration;
        public TransferBiz(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private readonly DbAccess access = new DbAccess();
        private async Task<int> IsExistTransection(string year, string month, string conString)
        {
            int result = 0;
            string? connectionString = _configuration.GetConnectionString(conString);
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("dbo.pr_IsExistData", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Month", month);
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
        private async Task<int> DeleteExistingTransectionData(string year, string month, string conString)
        {
            int result = 0;
            string? connectionString = _configuration.GetConnectionString(conString);
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("dbo.pr_DeleteExistingData", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Month", month);
                    result += command.ExecuteNonQuery();
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
        public async Task<string> CompanyCodeForTransection(long CompanyId)
        {
            string result = "";
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Shared.GetCodeForTransectionData", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@CompanyId", CompanyId);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        result = reader["EstateCode"].ToString()!;
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
        public async Task<int> SaveTransectionData(List<TransferData> transferDatas, string conString)
        {
            int result = 0;
            string? connectionString = _configuration.GetConnectionString(conString);
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    var isExist = IsExistTransection(transferDatas[0].Year, transferDatas[0].Month, conString).Result;
                    if (isExist > 0)
                    {
                        var deleteResult = DeleteExistingTransectionData(transferDatas[0].Year, transferDatas[0].Month, conString).Result;
                        if (deleteResult > 0)
                        {
                            result = InsertTransferData(transferDatas, conString).Result;
                        }
                    }
                    else
                    {
                        result = InsertTransferData(transferDatas, conString).Result;
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
        private async Task<int> InsertTransferData(List<TransferData> transferData, string conString)
        {
            int result = 0;
            string? connectionString = _configuration.GetConnectionString(conString);
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    foreach (var item in transferData)
                    {
                        SqlCommand command = new SqlCommand("dbo.pr_InsertTrialBalanceData", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@Year", item.Year);
                        command.Parameters.AddWithValue("@Month", item.Month);
                        command.Parameters.AddWithValue("@AccountNo", item.AccountNo);
                        command.Parameters.AddWithValue("@Description", item.Description);
                        command.Parameters.AddWithValue("@Crop", item.Crop);
                        command.Parameters.AddWithValue("@Amount", item.Amount);
                        result += command.ExecuteNonQuery();
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
                connection.Dispose();
            }
            return await Task.Run(() => result);
        }
        public async Task<List<Years>> GetYears()
        {
            List<Years> years = new List<Years>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Shared.GetYears", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        years.Add(new Years()
                        {
                            RecordId = Convert.ToInt64(reader["RecordId"]),
                            Year = reader["Year"].ToString(),
                            IsActive = Convert.ToInt32(reader["IsActive"])
                        });
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
            return await Task.Run(() => years);
        }
        public async Task<List<Months>> GetMonths()
        {
            List<Months> months = new List<Months>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Shared.GetMonths", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        months.Add(new Months()
                        {
                            RecordId = Convert.ToInt64(reader["RecordId"]),
                            MonthCode = reader["MonthCode"].ToString(),
                            MonthName = reader["MonthName"].ToString(),
                            IsActive = Convert.ToInt32(reader["IsActive"])
                        });
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
            return await Task.Run(() => months);
        }
        public async Task<List<TransferData>> GetTransferData(string year, string month, string conString)
        {
            List<TransferData> transferData = new List<TransferData>();
            string? connectionString = _configuration.GetConnectionString(conString);
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("dbo.pr_GetTrialBalanceByYearAndMonth", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@Month", month);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        transferData.Add(new TransferData()
                        {
                            Year = reader["Year"].ToString()!,
                            Month = reader["Month"].ToString()!,
                            AccountNo = reader["AccountNo"].ToString()!,
                            Description = reader["Description"].ToString()!,
                            Crop = reader["Crop"].ToString()!,
                            Amount = Convert.ToDouble(reader["Amount"])
                        });
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
            return await Task.Run(() => transferData);
        }
    }
}
