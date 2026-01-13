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
            foreach (var item in result)
            {
                ScholarshipData model = new ScholarshipData();
                model.StudentName = item.StudentName;
                model.ParentName = item.ParentName;
                model.DateOfBirth = item.DateOfBirth;
                model.BankName = item.BankName;
                model.BankBranch = item.BankBranch;
                model.BankAccountNo = item.BankAccountNo;
                model.BankRoutingNo = item.BankRoutingNo;
                double amount = 0;
                int eligibleMonths = 0;

                // Adjust amount based on age
                if (item.StAgeYears >= 7 && item.StAgeYears < 21)
                {
                    if(item.EmpEligibleMonths < 12)
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
                else if (item.StAgeYears >= 21 && item.StAgeYears <= 22) 
                {
                    if (item.EmpEligibleMonths < 12)
                    {
                        if(item.EmpEligibleDays >= 15)
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
                    if (item.StAgeMonths < item.EmpEligibleMonths) { 
                        eligibleMonths = item.StAgeMonths;
                    }
                    else
                    {
                        eligibleMonths = item.EmpEligibleMonths;
                    }
                }
                else if(item.StAgeYears == 6)
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
                model.Amount = amount;
                list.Add(model);
            }

            return list;
        }
    }
}
