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
    public class ExamResultBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public ExamResultBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<AssessmentYear>> GetAssessmentYearsAsync()
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
                        model.ShortCode = reader["ShortCode"].ToString();

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
        public async Task<int> SaveStudentResult(ExamResults model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.SaveStudentResult", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@StudentId", model.StudentId);
                    command.Parameters.AddWithValue("@ClassStudied", model.ClassStudied);
                    command.Parameters.AddWithValue("@NameOfTheInstitution", model.NameOfTheInstitution);
                    command.Parameters.AddWithValue("@StudyMedium", model.StudyMedium);
                    command.Parameters.AddWithValue("@AcademyType", model.AcademyType);
                    command.Parameters.AddWithValue("@ExamResult", model.ExamResult);
                    command.Parameters.AddWithValue("@AssessmentYear", model.AssessmentYear);                    
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
        public async Task<List<ExamResults>> GetExamResults()
        {
            List<ExamResults> list = new List<ExamResults>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetStudentResults", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ExamResults model = new ExamResults();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.ParentId = Convert.ToInt64(reader["ParentId"]);
                        model.StudentId = Convert.ToInt64(reader["StudentId"]);
                        model.StudentCode = reader["StudentCode"].ToString();
                        model.StudentName = reader["StudentName"].ToString();
                        model.ClassStudied = reader["ClassStudied"].ToString();
                        model.NameOfTheInstitution = reader["NameOfTheInstitution"].ToString();
                        model.StudyMedium = reader["StudyMedium"].ToString();
                        model.AcademyType = reader["AcademyType"].ToString();
                        model.ExamResult = reader["ExamResult"].ToString();
                        model.AssessmentYearId = Convert.ToInt64(reader["AssessmentYearId"]);
                        model.AssessmentYear = reader["AssessmentYear"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
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
        public async Task<int> UpdateExamResult(ExamResults model)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    SqlCommand command = new SqlCommand("Macalms.UpdateExamResult", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@RecordId", model.RecordId);
                    command.Parameters.AddWithValue("@StudentId", model.StudentId);
                    command.Parameters.AddWithValue("@ClassStudied", model.ClassStudied);
                    command.Parameters.AddWithValue("@NameOfTheInstitution", model.NameOfTheInstitution);
                    command.Parameters.AddWithValue("@StudyMedium", model.StudyMedium);
                    command.Parameters.AddWithValue("@AcademyType", model.AcademyType);
                    command.Parameters.AddWithValue("@ExamResult", model.ExamResult);
                    command.Parameters.AddWithValue("@AssessmentYear", model.AssessmentYear);

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
