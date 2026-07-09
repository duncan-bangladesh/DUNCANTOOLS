using dDataAccess;
using dVehicle.Model;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace dVehicle.Biz
{
    public class DriverBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public DriverBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<Drivers>> GetDriversAsync()
        {
            var list = new List<Drivers>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetDrivers", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Drivers
                {
                    RecordId = Convert.ToInt32(reader["RecordId"]),
                    DriverName = reader["DriverName"]?.ToString(),
                    CurrentAddress = reader["CurrentAddress"]?.ToString(),
                    MobileNo = reader["MobileNo"]?.ToString(),
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
