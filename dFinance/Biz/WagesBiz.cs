using dDataAccess;
using dFinance.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dFinance.Biz
{
    public class WagesBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public WagesBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        private async Task<List<WagesModel>> GetWagesData(DateTime FromDate, DateTime ToDate, string? ConString)
        {
            string? connectionString = _configuration.GetConnectionString(ConString!);
            SqlConnection connection = access.GetConnection(connectionString);
            List<WagesModel> list = new List<WagesModel>();
            SqlDataReader? reader = null;
            try
            {
                SqlCommand command = new SqlCommand("spGetWagesReportData", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@FromDate", FromDate);
                command.Parameters.AddWithValue("@ToDate", ToDate);

                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        WagesModel model = new WagesModel();
                        model.PermanentAttendance = Convert.ToInt32(reader["PermanentAttendance"]);
                        model.TemporaryAttendance = Convert.ToInt32(reader["TemporaryAttendance"]);
                        model.DoubleHazira = Convert.ToInt32(reader["DoubleHazira"]);
                        model.TotalAttendance = Convert.ToInt32(reader["TotalAttendance"]);
                        model.YTDPreviousMonth = Convert.ToInt32(reader["YTDPreviousMonth"]);
                        model.YTDAttendanceThisYear = Convert.ToInt32(reader["YTDAttendanceThisYear"]);
                        model.YTDAttendanceLastYear = Convert.ToInt32(reader["YTDAttendanceLastYear"]);
                        //model.AccountsCode = reader["AccountsCode"].ToString();
                        //model.AccountsDescription = reader["AccountsDescription"].ToString();
                        model.SubCode = reader["SubCode"].ToString();
                        model.SubCodeDescription = reader["SubCodeDescription"].ToString();
                        model.AccountsCategory = reader["AccountsCategory"].ToString();
                        model.AccountsHead = reader["AccountsHead"].ToString();
                        model.AccountsOrder = Convert.ToInt32(reader["AccountsOrder"]);
                        model.PermanentAttendanceWages = Convert.ToDouble(reader["PermanentAttendanceWages"]);
                        model.TemporaryAttendanceWages = Convert.ToDouble(reader["TemporaryAttendanceWages"]);
                        model.DoubleHaziraWages = Convert.ToDouble(reader["DoubleHaziraWages"]);
                        model.TotalAttendanceWages = Convert.ToDouble(reader["TotalAttendanceWages"]);
                        model.YTDWagesPreviousMonth = Convert.ToDouble(reader["YTDWagesPreviousMonth"]);
                        model.YTDWagesThisYear = Convert.ToDouble(reader["YTDWagesThisYear"]);
                        model.YTDWagesLastYear = Convert.ToDouble(reader["YTDWagesLastYear"]);

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
        //public async Task<List<WagesModel>> WagesReport(DateTime FromDate, DateTime ToDate, string? ConString, string? GardenCode)
        //{
        //    var result = new List<WagesModel>();
        //    var wagesData = await GetWagesData(FromDate, ToDate, ConString);
        //    var chartOfAccountsBiz = new ChartOfAccountsBiz(_configuration);
        //    var chartOfAccounts = await chartOfAccountsBiz.GetChartOfAccountsByGardenCode(GardenCode);

        //    var dataset = from chart in chartOfAccounts
        //                  join wage in wagesData
        //                  // Join on both conditions using an anonymous object
        //                  on new { SubGroup = chart.AccountsSubGroupCode, Category = chart.CostCenter }
        //                  equals new { SubGroup = wage.SubCode, Category = wage.AccountsCategory }
        //                  select new WagesModel
        //                  {
        //                      AccountsCode = chart.AccountsGroupCode,
        //                      AccountsDescription = chart.AccountsGroupDescription,
        //                      SubCode = chart.AccountsSubGroupCode,
        //                      SubCodeDescription = chart.SageAccountsDescription,

        //                      PermanentAttendance = wage.PermanentAttendance,
        //                      TemporaryAttendance = wage.TemporaryAttendance,
        //                      DoubleHazira = wage.DoubleHazira,
        //                      TotalAttendance = wage.TotalAttendance,
        //                      YTDPreviousMonth = wage.YTDPreviousMonth,
        //                      YTDAttendanceThisYear = wage.YTDAttendanceThisYear,
        //                      YTDAttendanceLastYear = wage.YTDAttendanceLastYear,
        //                      AccountsCategory = wage.AccountsCategory,
        //                      AccountsHead = wage.AccountsHead,
        //                      PermanentAttendanceWages = wage.PermanentAttendanceWages,
        //                      TemporaryAttendanceWages = wage.TemporaryAttendanceWages,
        //                      DoubleHaziraWages = wage.DoubleHaziraWages,
        //                      TotalAttendanceWages = wage.TotalAttendanceWages,
        //                      YTDWagesPreviousMonth = wage.YTDWagesPreviousMonth,
        //                      YTDWagesThisYear = wage.YTDWagesThisYear,
        //                      YTDWagesLastYear = wage.YTDWagesLastYear
        //                  };
        //    result = dataset.OrderByDescending(x => x.AccountsCategory).ThenBy(x => x.AccountsHead).ThenBy(x => x.AccountsDescription).ThenBy(x => x.SubCodeDescription).ToList();
        //    return await Task.Run(() => result);
        //}
        public async Task<List<WagesModel>> WagesReport(DateTime FromDate, DateTime ToDate, string? ConString, string? GardenCode)
        {
            var result = new List<WagesModel>();
            var wagesData = await GetWagesData(FromDate, ToDate, ConString);
            var chartOfAccountsBiz = new ChartOfAccountsBiz(_configuration);
            var chartOfAccounts = await chartOfAccountsBiz.GetChartOfAccountsByGardenCode(GardenCode);
            var parentAccounts = chartOfAccounts.Where(x => string.IsNullOrWhiteSpace(x.AccountsSubGroupCode)).ToList();
            foreach (var wage in wagesData)
            {
                var childAccounts = chartOfAccounts.Where(x => !string.IsNullOrWhiteSpace(x.AccountsSubGroupCode) && x.AccountsSubGroupCode == wage.SubCode && x.CostCenter == wage.AccountsCategory).ToList();
                foreach (var child in childAccounts)
                {
                    var parent = parentAccounts.FirstOrDefault(x => x.AccountsGroupCode == child.AccountsGroupCode);
                    if (parent == null)
                        continue;

                    var model = new WagesModel();
                    model.AccountsCode = parent.SageAccountsId;
                    model.AccountsDescription = parent.SageAccountsDescription;
                    model.SubCode = child.SageAccountsId;
                    model.SubCodeDescription = child.SageAccountsDescription;
                    model.AccountsCategory = wage.AccountsCategory;
                    model.AccountsHead = wage.AccountsHead;
                    model.AccountsOrder = wage.AccountsOrder;
                    model.PermanentAttendance = wage.PermanentAttendance;
                    model.TemporaryAttendance = wage.TemporaryAttendance;
                    model.DoubleHazira = wage.DoubleHazira;
                    model.TotalAttendance = wage.TotalAttendance;
                    model.YTDPreviousMonth = wage.YTDPreviousMonth;
                    model.YTDAttendanceThisYear = wage.YTDAttendanceThisYear;
                    model.YTDAttendanceLastYear = wage.YTDAttendanceLastYear;
                    model.PermanentAttendanceWages = wage.PermanentAttendanceWages;
                    model.TemporaryAttendanceWages = wage.TemporaryAttendanceWages;
                    model.DoubleHaziraWages = wage.DoubleHaziraWages;
                    model.TotalAttendanceWages = wage.TotalAttendanceWages;
                    model.YTDWagesPreviousMonth = wage.YTDWagesPreviousMonth;
                    model.YTDWagesThisYear = wage.YTDWagesThisYear;
                    model.YTDWagesLastYear = wage.YTDWagesLastYear;
                    result.Add(model);
                }
            }
            return await Task.Run(() => result.OrderByDescending(x=> x.AccountsOrder).ToList());
        }
    }
}

