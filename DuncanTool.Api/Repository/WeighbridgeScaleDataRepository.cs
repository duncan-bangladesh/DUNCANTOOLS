using dDataAccess;
using DuncanTool.Api.Model;
using DuncanTool.Api.Service;
using System.Data.SqlClient;
using System.Reflection;

namespace DuncanTool.Api.Repository
{
    public class WeighbridgeScaleDataRepository : IWeighbridgeScaleDataService
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public WeighbridgeScaleDataRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        
        public Task<List<WeighbridgeScaleData>> GetAllWeighbridgeScaleData()
        {
            throw new NotImplementedException();
        }
        public async Task<int> SaveScaleData(WeighbridgeScaleData model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Scale.InsertScaleData", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@SerialNo", model.SerialNo);
                    command.Parameters.AddWithValue("@VehicleId", model.VehicleId);
                    command.Parameters.AddWithValue("@VehicleNumber", model.VehicleNumber);
                    command.Parameters.AddWithValue("@MaterialId", model.MaterialId);
                    command.Parameters.AddWithValue("@Material", model.Material);
                    command.Parameters.AddWithValue("@CustomerId", model.CustomerId);
                    command.Parameters.AddWithValue("@Customer", model.Customer);
                    command.Parameters.AddWithValue("@Gross", model.Gross);
                    command.Parameters.AddWithValue("@Tare", model.Tare);
                    command.Parameters.AddWithValue("@Net", model.Net);
                    command.Parameters.AddWithValue("@RealNet", model.RealNet);
                    command.Parameters.AddWithValue("@RecordDateTime", model.RecordDateTime);
                    command.Parameters.AddWithValue("@SourceName", model.SourceName);
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Dispose();
            }
            return await Task.Run(() => result);
        }
        
    }
}
