using dDataAccess;
using Macalms.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Macalms.Biz
{
    public class StudentProfilBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public StudentProfilBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<string> GetStudentCodeByParentCode(string EmployeeRefCode)
        {
            string StudentCode = "";
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetStudentCodeByParentCode", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@EmployeeRefCode", EmployeeRefCode);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        StudentCode = reader["StudentCode"].ToString() ?? "";
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
            return await Task.Run(() => StudentCode);
        }
        public async Task<int> AddStudentProfile(StudentProfile model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.AddStudentProfile", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@ParentId", model.ParentId);
                    command.Parameters.AddWithValue("@StudentCode", model.StudentCode);
                    command.Parameters.AddWithValue("@StudentName", model.StudentName);
                    command.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", model.Gender);
                    command.Parameters.AddWithValue("@BankName", model.BankName);
                    command.Parameters.AddWithValue("@BankAccountNo", model.BankAccountNo);
                    command.Parameters.AddWithValue("@BankBranch", model.BankBranch);
                    command.Parameters.AddWithValue("@BankRoutingNo", model.BankRoutingNo);
                    //command.Parameters.AddWithValue("@EmployeeRefCode", model.EmployeeRefCode);                    
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
        public async Task<int> UpdateStudentProfile(StudentProfile model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.UpdateStudentProfile", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RecordId", model.RecordId);
                    command.Parameters.AddWithValue("@ParentId", model.ParentId);
                    command.Parameters.AddWithValue("@StudentCode", model.StudentCode);
                    command.Parameters.AddWithValue("@StudentName", model.StudentName);
                    command.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    command.Parameters.AddWithValue("@Gender", model.Gender);
                    command.Parameters.AddWithValue("@BankName", model.BankName);
                    command.Parameters.AddWithValue("@BankAccountNo", model.BankAccountNo);
                    command.Parameters.AddWithValue("@BankBranch", model.BankBranch);
                    command.Parameters.AddWithValue("@BankRoutingNo", model.BankRoutingNo);

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
        public async Task<int> ChangeStudentStatus(StudentProfile model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.ChangeStudentStatus", connection);
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
        public async Task<List<StudentProfile>> GetStudentProfiles()
        {
            List<StudentProfile> list = new List<StudentProfile>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetStudentProfile", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        StudentProfile model = new StudentProfile();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.ParentId = Convert.ToInt64(reader["ParentId"]);
                        model.ParentName = reader["ParentName"].ToString();
                        model.StudentCode = reader["StudentCode"].ToString();
                        model.StudentName = reader["StudentName"].ToString();
                        model.DateOfBirth = reader["DateOfBirth"].ToString();
                        model.Gender = reader["Gender"].ToString();
                        model.BankId = Convert.ToInt64(reader["BankId"]);
                        model.BankName = reader["BankName"].ToString();
                        model.BankAccountNo = reader["BankAccountNo"].ToString();
                        model.BankBranch = reader["BankBranch"].ToString();
                        model.BankRoutingNo = reader["BankRoutingNo"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.ScholarshipStatus = reader["ScholarshipStatus"].ToString();
                        model.EmployeeRefCode = reader["EmployeeRefCode"].ToString();

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
