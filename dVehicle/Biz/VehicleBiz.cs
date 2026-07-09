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
    public class VehicleBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public VehicleBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<int> AddVehicleAsync(Vehicles model)
        {
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.AddVehicle", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@VehicleName", model.VehicleName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@OwnerId", model.OwnerId);
            command.Parameters.AddWithValue("@RegistrationDate", model.RegistrationDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LicensePlate", model.LicensePlate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IssueLocationId", model.IssueLocationId);
            command.Parameters.AddWithValue("@IssueToId", model.IssueToId);
            command.Parameters.AddWithValue("@VehicleTypeId", model.VehicleTypeId);
            command.Parameters.AddWithValue("@BRTAOfficeId", model.BRTAOfficeId);
            command.Parameters.AddWithValue("@DriverId", model.DriverId);
            command.Parameters.AddWithValue("@SeatCapacityWithDriver", model.SeatCapacityWithDriver);
            command.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EntryBy", model.EntryBy);
            return await command.ExecuteNonQueryAsync();
        }
        public async Task<int> UpdateVehicleAsync(Vehicles model)
        {
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.UpdateVehicles", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@RecordId", model.RecordId);
            command.Parameters.AddWithValue("@VehicleName", model.VehicleName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@OwnerId", model.OwnerId);
            command.Parameters.AddWithValue("@RegistrationDate", model.RegistrationDate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LicensePlate", model.LicensePlate ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@IssueLocationId", model.IssueLocationId);
            command.Parameters.AddWithValue("@IssueToId", model.IssueToId);
            command.Parameters.AddWithValue("@VehicleTypeId", model.VehicleTypeId);
            command.Parameters.AddWithValue("@BRTAOfficeId", model.BRTAOfficeId);
            command.Parameters.AddWithValue("@DriverId", model.DriverId);
            command.Parameters.AddWithValue("@SeatCapacityWithDriver", model.SeatCapacityWithDriver);
            command.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EntryBy", model.EntryBy ?? (object)DBNull.Value);
            return await command.ExecuteNonQueryAsync();
        }
        public async Task<List<Vehicles>> GetVehiclesAsync()
        {
            var vehicles = new List<Vehicles>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetVehicles", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                vehicles.Add(new Vehicles
                {
                    RecordId = Convert.ToInt64(reader["RecordId"]),
                    VehicleName = reader["VehicleName"]?.ToString(),                    
                    RegistrationDate = reader["RegistrationDate"]?.ToString(),
                    LicensePlate = reader["LicensePlate"]?.ToString(),
                    OwnerId = Convert.ToInt64(reader["OwnerId"]),
                    OwnerName = reader["OwnerName"]?.ToString(),
                    IssueLocationId = Convert.ToInt64(reader["IssueLocationId"]),
                    IssueLocationName = reader["LocationName"]?.ToString(),
                    CompanyId = Convert.ToInt32(reader["CompanyId"]),
                    IssueToId = Convert.ToInt64(reader["IssueToId"]),
                    IssueToName = reader["ReceiverName"]?.ToString(),
                    VehicleTypeId = Convert.ToInt32(reader["VehicleTypeId"]),
                    VehicleTypeName = reader["TypeName"]?.ToString(),
                    BRTAOfficeId = Convert.ToInt32(reader["BRTAOfficeId"]),
                    BRTAOfficeName = reader["OfficeName"]?.ToString(),
                    DriverId = Convert.ToInt32(reader["DriverId"]),
                    DriverName = reader["DriverName"]?.ToString(),
                    SeatCapacityWithDriver = Convert.ToInt32(reader["SeatCapacityWithDriver"]),
                    Remarks = reader["Remarks"]?.ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }
            return vehicles;
        }
    }
}
