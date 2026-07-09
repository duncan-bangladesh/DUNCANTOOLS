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
    public class DocumentTypeBiz
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public DocumentTypeBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<DocumentType>> GetDocumentTypeAsync()
        {
            var list = new List<DocumentType>();
            await using var connection = access.GetConnection(connectionString);
            await using var command = new SqlCommand("Vehicle.GetDocumentType", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new DocumentType
                {
                    RecordId = Convert.ToInt32(reader["RecordId"]),
                    DocumentTypeName = reader["DocumentTypeName"]?.ToString(),
                    DocumentTypeDescription = reader["DocumentTypeDescription"]?.ToString(),
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
