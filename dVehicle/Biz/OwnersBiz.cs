using dDataAccess;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using dVehicle.Model;

namespace dVehicle.Biz
{
    public class OwnersBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public OwnersBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<Owners>> GetOwnersAsync()
        {
            var list = new List<Owners>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetOwners", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();            
            while (await reader.ReadAsync())
            {
                list.Add(new Owners
                {
                    RecordId = Convert.ToInt32(reader["RecordId"]),
                    OwnerName = reader["OwnerName"]?.ToString(),
                    OwnerDescription = reader["OwnerDescription"]?.ToString(),
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
