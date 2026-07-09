using dDataAccess;
using dVehicle.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dVehicle.Biz
{
    public class IssueToBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public IssueToBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<IssueTo>> GetIssueToAsync()
        {
            var list = new List<IssueTo>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetIssueTo", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();            
            while (await reader.ReadAsync())
            {
                list.Add(new IssueTo
                {
                    RecordId = Convert.ToInt64(reader["RecordId"]),
                    ReceiverName = reader["ReceiverName"]?.ToString(),
                    MobileNo = reader["MobileNo"]?.ToString(),
                    EmailAddress = reader["EmailAddress"]?.ToString(),
                    CurrentAddress = reader["CurrentAddress"]?.ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    EntryBy = reader["EntryBy"]?.ToString(),
                    EntryDate = reader["EntryDate"]?.ToString(),
                    ModifyBy = reader["ModifyBy"]?.ToString(),
                    ModifyDate = reader["ModifyDate"]?.ToString()
                });
            }
            return list;
        }
    }
}
