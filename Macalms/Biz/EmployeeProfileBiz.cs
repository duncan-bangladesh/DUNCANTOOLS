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
    public class EmployeeProfileBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public EmployeeProfileBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<int> AddEmployeeProfile(EmployeeProfile model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.AddEmployeeProfile", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@EmployeeCode", model.EmployeeCode);
                    command.Parameters.AddWithValue("@EmployeeName", model.EmployeeName);
                    command.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                    command.Parameters.AddWithValue("@DesignationId", model.DesignationId);
                    command.Parameters.AddWithValue("@WorkLocationId", model.WorkLocationId);
                    command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                    command.Parameters.AddWithValue("@ContactNumber", model.ContactNumber);
                    command.Parameters.AddWithValue("@ApplicableFrom", model.ApplicableFrom);
                    command.Parameters.AddWithValue("@ApplicableUpto", model.ApplicableUpto);
                    command.Parameters.AddWithValue("@EntryBy", model.EntryBy);
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
        public async Task<int> UpdateEmployeeProfile(EmployeeProfile model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.UpdateEmployeeProfile", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RecordId", model.RecordId);
                    command.Parameters.AddWithValue("@EmployeeCode", model.EmployeeCode);
                    command.Parameters.AddWithValue("@EmployeeName", model.EmployeeName);
                    command.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
                    command.Parameters.AddWithValue("@DesignationId", model.DesignationId);
                    command.Parameters.AddWithValue("@WorkLocationId", model.WorkLocationId);
                    command.Parameters.AddWithValue("@EmailAddress", model.EmailAddress);
                    command.Parameters.AddWithValue("@ContactNumber", model.ContactNumber);
                    command.Parameters.AddWithValue("@ApplicableFrom", model.ApplicableFrom);
                    command.Parameters.AddWithValue("@ApplicableUpto", model.ApplicableUpto);
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
        public async Task<List<EmployeeProfile>> GetEmployeeProfiles()
        {
            List<EmployeeProfile> list = new List<EmployeeProfile>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetEmployeeProfile", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        EmployeeProfile model = new EmployeeProfile();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.EmployeeCode = reader["EmployeeCode"].ToString();
                        model.EmployeeName = reader["EmployeeName"].ToString();
                        model.DepartmentId = Convert.ToInt64(reader["DepartmentId"]);
                        model.DepartmentName = reader["DepartmentName"].ToString();
                        model.DesignationId = Convert.ToInt64(reader["DesignationId"]);
                        model.DesignationName = reader["DesignationName"].ToString();
                        model.WorkLocationId = Convert.ToInt64(reader["WorkLocationId"]);
                        model.WorkingIn = reader["LocationName"].ToString();
                        model.ContactNumber = reader["ContactNumber"].ToString();
                        model.EmailAddress = reader["EmailAddress"].ToString();
                        //model.JobStatus = reader["JobStatus"].ToString();
                        model.ApplicableFrom = reader["ApplicableFrom"].ToString();
                        model.ApplicableUpto = reader["ApplicableUpto"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        //model.EntryBy = reader["EntryBy"].ToString();
                        //model.EntryDate = reader["EntryDate"].ToString();
                        //model.ModifyBy = reader["ModifyBy"].ToString();
                        //model.ModifyDate = reader["ModifyDate"].ToString();

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
        public async Task<int> ChangeEmployeeStatus(EmployeeProfile model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.ChangeEmployeeStatus", connection);
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
