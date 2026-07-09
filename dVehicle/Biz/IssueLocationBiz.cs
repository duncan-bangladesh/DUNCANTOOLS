using dDataAccess;
using dVehicle.Model;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace dVehicle.Biz
{
    public class IssueLocationBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public IssueLocationBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<IssueLocation>> GetIssueLocationAsync()
        {
            var list = new List<IssueLocation>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetIssueLocation", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();            
            while (await reader.ReadAsync())
            {
                list.Add(new IssueLocation
                {
                    RecordId = Convert.ToInt64(reader["RecordId"]),
                    LocationName = reader["LocationName"]?.ToString(),
                    LocationDescription = reader["LocationDescription"]?.ToString(),
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
