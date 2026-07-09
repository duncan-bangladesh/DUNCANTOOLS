using dDataAccess;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using dVehicle.Model;

namespace dVehicle.Biz
{
    public class BRTAOfficeBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public BRTAOfficeBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<BRTAOffice>> GetBRTAOfficeAsync()
        {
            var list = new List<BRTAOffice>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetBRTAOffice", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();            
            while (await reader.ReadAsync())
            {
                list.Add(new BRTAOffice
                {
                    RecordId = Convert.ToInt32(reader["RecordId"]),
                    OfficeName = reader["OfficeName"]?.ToString(),
                    OfficeAddress = reader["OfficeAddress"]?.ToString(),
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
