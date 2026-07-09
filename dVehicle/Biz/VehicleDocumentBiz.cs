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
    public class VehicleDocumentBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public VehicleDocumentBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<int> SaveVehicleDocument(VehicleDocument model)
        {
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.SaveDocument", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@DocumentTypeId", model.DocumentTypeId);
            command.Parameters.AddWithValue("@VehicleId", model.VehicleId);
            command.Parameters.Add("@IssueDate", SqlDbType.Date).Value = model.IssueDate.Date;
            command.Parameters.Add("@ExpiredDate", SqlDbType.Date).Value = model.ExpiredDate.Date;
            command.Parameters.AddWithValue("@FilePath", model.FilePath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@EntryBy", model.EntryBy);
            return await command.ExecuteNonQueryAsync();
        }
        public async Task<List<VehicleDocument>> GetVehicleDocuments()
        {
            var list = new List<VehicleDocument>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetVehicleDocuments", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();
            
            string basePath = _configuration["ApplicationConfig:VehicleAttachmentPath"]!;
            while (await reader.ReadAsync())
            {
                var relativePath = reader["FilePath"]?.ToString();

                string physicalPath = string.Empty;

                if (!string.IsNullOrEmpty(relativePath))
                {
                    physicalPath = Path.Combine(
                        basePath,
                        relativePath.Replace("/", "\\")
                    );
                }
                list.Add(new VehicleDocument
                {

                    RecordId = Convert.ToInt64(reader["RecordId"]),
                    DocumentTypeId = Convert.ToInt32(reader["DocumentTypeId"]),
                    DocumentTypeName = reader["DocumentTypeName"]?.ToString(),
                    VehicleId = Convert.ToInt32(reader["VehicleId"]),
                    VehicleName = reader["VehicleName"]?.ToString(),
                    IssueLocationId = Convert.ToInt64(reader["IssueLocationId"]),
                    CompanyId = Convert.ToInt32(reader["CompanyId"]),
                    IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                    ExpiredDate = Convert.ToDateTime(reader["ExpiredDate"]),
                    //FilePath = reader["FilePath"]?.ToString(),
                    FilePath = relativePath,
                    FileUrl = string.IsNullOrEmpty(relativePath) ? null : "/Vehicle/Download?path=" + Uri.EscapeDataString(relativePath),
                    PhysicalPath = physicalPath,
                    Remarks = reader["Remarks"]?.ToString(),
                    EntryBy = reader["EntryBy"]?.ToString(),
                    EntryDate = reader["EntryDate"]?.ToString(),
                    ModifyBy = reader["ModifyBy"]?.ToString(),
                    ModifyDate = reader["ModifyDate"]?.ToString()
                });
            }
            return list;
        }
        public async Task<int> UpdateVehicleDocumentAsync(VehicleDocument model)
        {
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.UpdateDocument", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@RecordId", model.RecordId);
            command.Parameters.AddWithValue("@DocumentTypeId", model.DocumentTypeId);
            command.Parameters.AddWithValue("@VehicleId", model.VehicleId);
            command.Parameters.AddWithValue("@IssueDate", model.IssueDate);
            command.Parameters.AddWithValue("@ExpiredDate", model.ExpiredDate);
            command.Parameters.AddWithValue("@FilePath", model.FilePath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Remarks", model.Remarks ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ModifyBy", model.ModifyBy ?? (object)DBNull.Value);
            return await command.ExecuteNonQueryAsync();
        }
    }
}
