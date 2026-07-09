using dDataAccess;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using dVehicle.Model;

namespace dVehicle.Biz
{
    public class VehicleTypeBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public VehicleTypeBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<VehicleType>> GetVehicleTypeAsync()
        {
            var list = new List<VehicleType>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetVehicleType", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();            
            while (await reader.ReadAsync())
            {
                list.Add(new VehicleType
                {
                    RecordId = Convert.ToInt32(reader["RecordId"]),
                    TypeName = reader["TypeName"]?.ToString(),
                    TypeDescription = reader["TypeDescription"]?.ToString(),
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
