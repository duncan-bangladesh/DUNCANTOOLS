using dDataAccess;
using Macalms.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macalms.Biz
{
    public class ScholarshipBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public ScholarshipBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        private async Task<List<Scholarships>> GetAllEligibleStudent(int AssessmentYear)
        {
            List<Scholarships> list = new List<Scholarships>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetAllEligibleStudent", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@AssessmentYear", AssessmentYear);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Scholarships model = new Scholarships();
                        model.StudentCode = reader["StudentCode"].ToString();
                        model.StudentName = reader["StudentName"].ToString();
                        model.ParentName = reader["ParentName"].ToString();
                        model.DateOfBirth = reader["DateOfBirth"].ToString();
                        model.StudyMedium = reader["StudyMedium"].ToString();
                        model.StAgeYears = Convert.ToInt32(reader["StAgeYears"]);
                        model.StAgeMonths = Convert.ToInt32(reader["StAgeMonths"]);
                        model.StAgeDays = Convert.ToInt32(reader["StAgeDays"]);
                        model.BankName = reader["BankName"].ToString();
                        model.BankBranch = reader["BankBranch"].ToString();
                        model.BankAccountNo = reader["BankAccountNo"].ToString();
                        model.BankRoutingNo = reader["BankRoutingNo"].ToString();
                        model.EmpEligibleMonths = Convert.ToInt32(reader["EmpEligibleMonths"]);
                        model.EmpEligibleDays = Convert.ToInt32(reader["EmpEligibleDays"]);
                        model.EmpEligibleDays = Convert.ToInt32(reader["EmpEligibleDays"]);
                        model.IsPayment = Convert.ToInt32(reader["IsPayment"]);

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
        public async Task<List<ScholarshipData>> GetScholarshipData(int AssessmentYear)
        {
            List<ScholarshipData> list = new List<ScholarshipData>();
            var result = await GetAllEligibleStudent(AssessmentYear);
            int serialNo = 0;
            foreach (var item in result)
            {
                serialNo = serialNo + 1;
                ScholarshipData model = new ScholarshipData();
                model.SL = serialNo;
                model.AssessmentYear = AssessmentYear;
                model.StudentCode = item.StudentCode;
                model.StudentName = item.StudentName;
                model.StudyMedium = item.StudyMedium;
                model.ParentName = item.ParentName;
                model.DateOfBirth = item.DateOfBirth;
                model.BankName = item.BankName;
                model.BankBranch = item.BankBranch;
                model.BankAccountNo = item.BankAccountNo;
                model.BankRoutingNo = item.BankRoutingNo;
                model.Age = Ages(item.StAgeYears, item.StAgeMonths, item.StAgeDays);
                model.IsPayment = item.IsPayment;
                double amount = 0;
                int eligibleMonths = 0;

                // Adjust amount based on age
                if (item.StAgeYears >= 7 && item.StAgeYears < 21)
                {
                    if (item.EmpEligibleMonths < 12)
                    {
                        if (item.EmpEligibleDays >= 15)
                        {
                            item.EmpEligibleMonths = item.EmpEligibleMonths + 1;
                        }
                        else
                        {
                            item.EmpEligibleMonths = item.EmpEligibleMonths;
                        }
                    }
                    eligibleMonths = item.EmpEligibleMonths;
                }
                else if (item.StAgeYears >= 21 && item.StAgeYears < 22)
                {
                    if (item.EmpEligibleMonths < 12)
                    {
                        if (item.EmpEligibleDays >= 15)
                        {
                            item.EmpEligibleMonths = item.EmpEligibleMonths + 1;
                        }
                        else
                        {
                            item.EmpEligibleMonths = item.EmpEligibleMonths;
                        }
                    }
                    if (item.StAgeMonths < 12)
                    {
                        if (item.StAgeDays >= 15)
                        {
                            item.StAgeMonths = item.StAgeMonths + 1;
                        }
                        else
                        {
                            item.StAgeMonths = item.StAgeMonths;
                        }
                    }
                    if (item.StAgeMonths < item.EmpEligibleMonths)
                    {
                        eligibleMonths = item.StAgeMonths;
                    }
                    else
                    {
                        eligibleMonths = item.EmpEligibleMonths;
                    }
                    eligibleMonths = 12 - eligibleMonths;
                }
                else if (item.StAgeYears == 6)
                {
                    if (item.EmpEligibleMonths < 12)
                    {
                        if (item.EmpEligibleDays >= 15)
                        {
                            item.EmpEligibleMonths = item.EmpEligibleMonths + 1;
                        }
                        else
                        {
                            item.EmpEligibleMonths = item.EmpEligibleMonths;
                        }
                    }
                    if (item.StAgeMonths < 12)
                    {
                        if (item.StAgeDays >= 15)
                        {
                            item.StAgeMonths = item.StAgeMonths + 1;
                        }
                        else
                        {
                            item.StAgeMonths = item.StAgeMonths;
                        }
                    }
                    if (item.StAgeMonths < item.EmpEligibleMonths)
                    {
                        eligibleMonths = item.StAgeMonths;
                    }
                    else
                    {
                        eligibleMonths = item.EmpEligibleMonths;
                    }
                }

                // Base amount, based on study medium
                if (item.StudyMedium == "English")
                {
                    amount = 2500 * eligibleMonths;
                }
                else if (item.StudyMedium == "Bengali")
                {
                    amount = 2000 * eligibleMonths;
                }

                model.ScholarshipDuration = eligibleMonths;
                model.AllowedMonths = 0;
                model.Amount = amount;
                list.Add(model);
            }

            return list;
        }

        public async Task<List<ScholarshipData>> GetScholarshipPaymentByAssessmentYear(int AssessmentYear)
        {
            List<ScholarshipData> list = new List<ScholarshipData>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetScholarshipPayments", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@AssessmentYear", AssessmentYear);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ScholarshipData model = new ScholarshipData();
                        model.SL = Convert.ToInt32(reader["SL"]);
                        model.AssessmentYear = Convert.ToInt32(reader["AssessmentYear"]);
                        model.StudentCode = reader["StudentCode"].ToString();
                        model.StudentName = reader["StudentName"].ToString();
                        model.StudyMedium = reader["StudyMedium"].ToString();
                        model.ParentName = reader["ParentName"].ToString();
                        model.DateOfBirth = reader["DateOfBirth"].ToString();
                        model.Age = reader["Age"].ToString();
                        model.ScholarshipDuration = Convert.ToInt32(reader["ScholarshipDuration"]);
                        model.AllowedMonths = Convert.ToInt32(reader["AllowedMonths"]);
                        model.BankName = reader["BankName"].ToString();
                        model.BankBranch = reader["BankBranch"].ToString();
                        model.BankAccountNo = reader["BankAccountNo"].ToString();
                        model.BankRoutingNo = reader["BankRoutingNo"].ToString();
                        model.Amount = Convert.ToDouble(reader["Amount"]);
                        model.IsPayment = 1;
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

        public async Task<int> SaveScholarshipPayment(List<Payment>? data, string EntryBy)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                foreach (var item in data!)
                {
                    SqlCommand command = new SqlCommand("Macalms.SaveScholarshipPayment", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@SL", item.SL);
                    command.Parameters.AddWithValue("@AssessmentYear", item.AssessmentYear);
                    command.Parameters.AddWithValue("@StudentCode", item.StudentCode);
                    command.Parameters.AddWithValue("@StudentName", item.StudentName);
                    command.Parameters.AddWithValue("@StudyMedium", item.StudyMedium);
                    command.Parameters.AddWithValue("@ParentName", item.ParentName);
                    command.Parameters.AddWithValue("@DateOfBirth", item.DateOfBirth);
                    command.Parameters.AddWithValue("@Age", item.Age);
                    command.Parameters.AddWithValue("@ScholarshipDuration", item.ScholarshipDuration);
                    command.Parameters.AddWithValue("@AllowedMonths", item.AllowedMonths);
                    command.Parameters.AddWithValue("@BankName", item.BankName);
                    command.Parameters.AddWithValue("@BankBranch", item.BankBranch);
                    command.Parameters.AddWithValue("@BankAccountNo", item.BankAccountNo);
                    command.Parameters.AddWithValue("@BankRoutingNo", item.BankRoutingNo);
                    command.Parameters.AddWithValue("@Amount", item.Amount);
                    command.Parameters.AddWithValue("@EntryBy", EntryBy);
                    result = command.ExecuteNonQuery();
                    command.Dispose();
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
        private string Ages(int year, int month, int day)
        {
            return $"{year:00}y {month:00}m {day:00}d";
        }
    }
}
