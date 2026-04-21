using dDataAccess;
using dWeighbridge.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace dWeighbridge.Biz
{
    public class ScaleDataBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public ScaleDataBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<ScaleData>> FilterScaleData(string? EstateCode, string? FromDate, string? ToDate)
        {
            List<ScaleData> list = new List<ScaleData>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Scale.FilterScaleData", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@EstateCode", EstateCode);
                command.Parameters.AddWithValue("@FromDate", FromDate);
                command.Parameters.AddWithValue("@ToDate", ToDate);

                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ScaleData model = new ScaleData();
                        model.RecordDate = reader["RecordDate"].ToString();
                        model.RecordTime = reader["RecordTime"].ToString();
                        model.Vehicle = reader["Vehicle"].ToString();
                        model.LoadedVehicle = Convert.ToDecimal(reader["LoadedVehicle"]);
                        model.TareWeight = Convert.ToDecimal(reader["TareWeight"]);
                        model.NetWeight = Convert.ToDecimal(reader["NetWeight"]);
                        model.TeaEstate = reader["TeaEstate"].ToString();

                        list.Add(model);
                    }
                }
                connection.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                connection.Dispose();
            }
            return await Task.Run(() => list);
        }
    }
}
