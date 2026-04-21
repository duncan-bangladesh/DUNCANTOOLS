using dDataAccess;
using Macalms.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalms.Biz
{
    public class AssessmentYearBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public AssessmentYearBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<AssessmentYear>> GetAssessmentYears()
        {
            List<AssessmentYear> list = new List<AssessmentYear>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetAssessmentYear", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        AssessmentYear model = new AssessmentYear();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.YearName = reader["YearName"].ToString();
                        model.IsActive = Convert.ToInt32(reader["IsActive"]);

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
        public async Task<int> SaveAssessmentYear(AssessmentYear model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.SaveAssessmentYear", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@YearName", model.YearName);
                    command.Parameters.AddWithValue("@EntryBy", model.EntryBy);
                    result = Convert.ToInt32(command.ExecuteScalar());
                }
                connection.Close();
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
        public async Task<int> ChangeYearsStatus(AssessmentYear model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.ChangeYearsStatus", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RecordId", model.RecordId);
                    command.Parameters.AddWithValue("@IsActive", model.IsActive);
                    command.Parameters.AddWithValue("@ModifyBy", model.ModifyBy);
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
