using dDataAccess;
using dFinance.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dFinance.Biz
{
    public class ChartOfAccountsBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string? connectionString;
        public ChartOfAccountsBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("Dev_Tools");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<ChartOfAccounts>> GetChartOfAccountsByGardenCode(string? GardenCode)
        {            
            SqlConnection connection = access.GetConnection(connectionString);
            List<ChartOfAccounts> list = new List<ChartOfAccounts>();
            SqlDataReader? reader = null;
            try
            {
                SqlCommand command = new SqlCommand("Finance.GetGardenWiseAccCodes", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@LocationCode", GardenCode);

                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ChartOfAccounts model = new ChartOfAccounts();
                        model.SageAccountsId = reader["SageAccountsId"].ToString();
                        model.SageAccountsDescription = reader["SageAccountsDescription"].ToString();
                        model.CostCenter = reader["CostCenter"].ToString();
                        model.LocationCode = reader["LocationCode"].ToString();
                        model.AccountsGroupCode = reader["AccountsGroupCode"].ToString();
                        model.AccountsGroupDescription = reader["AccountsGroupDescription"].ToString();
                        model.AccountsSubGroupCode = reader["AccountsSubGroupCode"].ToString();
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
