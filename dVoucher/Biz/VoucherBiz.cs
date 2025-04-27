using dDataAccess;
using dVoucher.Model;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace dVoucher.Biz
{
    public class VoucherBiz
    {
        private readonly IConfiguration _configuration;
        private string? connectionString;
        public VoucherBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private DbAccess access = new DbAccess();
        private int CheckIsValidGLCode(string? glCode)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.IsValidGLCode", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@AccCode", glCode);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt32(reader["IsExist"]);
                }
                reader.Close();
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
            return result;
        }
        private int CheckIsValidAccCode(string? accCode)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.IsValidAccCode", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@AccCode", accCode);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt32(reader["IsExist"]);
                }
                reader.Close();
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
            return result;
        }
        private long GetFraGetVoucherErrorLogId()
        {
            long result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.GetFraGetVoucherErrorLogId", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt64(reader["LogId"]);
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
            return result;
        }
        public int InsertVoucherToFRATool(VoucherMaster master, string divisionCode, string UserName) 
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (master!=null)
                {
                    var countGlCodeError = 0;
                    var countAccCodeError = 0;
                    List<TempVoucherData> tempData = new List<TempVoucherData>();
                    List<TempVoucherDetail> tempDetails = new List<TempVoucherDetail>();
                    VoucherMaster vMaster = master;
                    foreach(VoucherData data in vMaster.data!)
                    {
                        //
                        if (CheckIfExistVoucher(vMaster.date, vMaster.estate, data.division, data.voucher_type) == 0)
                        {
                            foreach (VoucherDetail vDetail in data.data!)
                            {
                                if (CheckIsValidGLCode(vDetail.account_code) == 0)
                                {
                                    TempVoucherData tempVoucherData = new TempVoucherData()
                                    {
                                        AccountCode = vDetail.account_code,
                                        HeadName = vDetail.head_name,
                                        Description = vDetail.description,
                                        Amount = vDetail.amount
                                    };
                                    tempData.Add(tempVoucherData);
                                    countGlCodeError++;
                                }
                                foreach (VoucherDetail detail in vDetail.data!)
                                {
                                    if (CheckIsValidAccCode(detail.account_code) == 0)
                                    {
                                        if (detail.account_code != vDetail.account_code)
                                        {
                                            TempVoucherDetail tempVoucherDetail = new TempVoucherDetail()
                                            {
                                                GlCode = vDetail.account_code,
                                                AccountCode = detail.account_code,
                                                HeadName = detail.head_name,
                                                Description = detail.description,
                                                Amount = detail.amount
                                            };
                                            tempDetails.Add(tempVoucherDetail);
                                            countAccCodeError++;
                                        }
                                    }
                                }
                            }
                            if (countGlCodeError > 0 || countAccCodeError > 0)
                            {
                                //Insert to Error-Log Table
                                List<TempErrorLog> errorLogs = new List<TempErrorLog>();
                                long LogId = GetFraGetVoucherErrorLogId();
                                foreach(TempVoucherData voucherData in tempData)
                                {
                                    var tempDetail = tempDetails.FindAll(x => x.GlCode == voucherData.AccountCode);
                                    foreach (TempVoucherDetail voucherDetail in tempDetail)
                                    {
                                        TempErrorLog errorLog = new TempErrorLog()
                                        {
                                            LogId = LogId,
                                            GlCode = voucherData.AccountCode,
                                            GlDescription = voucherData.HeadName,
                                            AccCode = voucherDetail.AccountCode,
                                            AccDescription = voucherDetail.Description,
                                            Amount = Convert.ToDouble(voucherDetail.Amount),
                                            VoucherDate = vMaster.date,
                                            Estate = vMaster.estate,
                                            DivisionCode = divisionCode,
                                            VoucherType = data.voucher_type,
                                            EntryBy = UserName
                                        };
                                        errorLogs.Add(errorLog);
                                    }
                                }
                                SaveFraGetVoucherErrorLog(errorLogs);
                                result = 99;
                            }
                            else
                            {
                                SqlCommand command = new SqlCommand("Voucher.SaveVoucherMaster", connection);
                                command.CommandType = System.Data.CommandType.StoredProcedure;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@date", vMaster.date);
                                command.Parameters.AddWithValue("@company", vMaster.company);
                                command.Parameters.AddWithValue("@divisionCode", divisionCode);
                                command.Parameters.AddWithValue("@estate", vMaster.estate);
                                command.Parameters.AddWithValue("@division", data.division);
                                command.Parameters.AddWithValue("@description", data.description);
                                command.Parameters.AddWithValue("@voucher_type", data.voucher_type);
                                command.Parameters.Add(
                                    new SqlParameter()
                                    {
                                        Direction = ParameterDirection.Output,
                                        ParameterName = "@MasterId",
                                        SqlDbType = SqlDbType.BigInt
                                    }
                                );
                                if (command.ExecuteNonQuery() > 0)
                                {
                                    result = 1;
                                    long MasterId = Convert.ToInt64(command.Parameters["@MasterId"].Value);
                                    command.Dispose();
                                    if (MasterId > 0)
                                    {
                                        foreach (VoucherDetail vDetail in data.data!)
                                        {
                                            command = new SqlCommand("Voucher.SaveVoucherData", connection);
                                            command.CommandType = System.Data.CommandType.StoredProcedure;
                                            command.Parameters.Clear();
                                            command.Parameters.AddWithValue("@MasterId", MasterId);
                                            command.Parameters.AddWithValue("@account_code", vDetail.account_code);
                                            command.Parameters.AddWithValue("@head_name", vDetail.head_name);
                                            command.Parameters.AddWithValue("@description", vDetail.description);
                                            command.Parameters.AddWithValue("@amount", vDetail.amount);
                                            command.Parameters.Add(
                                                new SqlParameter()
                                                {
                                                    Direction = ParameterDirection.Output,
                                                    ParameterName = "@DataId",
                                                    SqlDbType = SqlDbType.BigInt
                                                }
                                            );
                                            if (command.ExecuteNonQuery() > 0)
                                            {
                                                result = 1;
                                                long DataId = Convert.ToInt64(command.Parameters["@DataId"].Value);
                                                command.Dispose();
                                                if (DataId > 0)
                                                {
                                                    foreach (VoucherDetail datas in vDetail.data!)
                                                    {
                                                        if (datas.account_code != vDetail.account_code)
                                                        {
                                                            command = new SqlCommand("Voucher.SaveVoucherDetails", connection);
                                                            command.CommandType = System.Data.CommandType.StoredProcedure;
                                                            command.Parameters.Clear();
                                                            command.Parameters.AddWithValue("@DataId", DataId);
                                                            command.Parameters.AddWithValue("@account_code", datas.account_code);
                                                            command.Parameters.AddWithValue("@head_name", datas.head_name);
                                                            command.Parameters.AddWithValue("@description", datas.description);
                                                            command.Parameters.AddWithValue("@amount", datas.amount);
                                                            if (command.ExecuteNonQuery() > 0)
                                                            {
                                                                result = 1;
                                                            }
                                                            else
                                                            {
                                                                result = 0;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                result = 0;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    result = 0;
                                }
                            }
                        }
                        else
                        {
                            result = 2;
                        }
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
                connection.Dispose();
            }
            return result;
        }
        public int SaveVoucherMaster(VoucherMaster master, string divisionCode)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (master != null)
                {
                    VoucherMaster vMaster = master;
                    foreach (VoucherData data in vMaster.data!)
                    {
                        if (CheckIfExistVoucher(vMaster.date, vMaster.estate, data.division, data.voucher_type) == 0)
                        {
                            SqlCommand command = new SqlCommand("Voucher.SaveVoucherMaster", connection);
                            command.CommandType = System.Data.CommandType.StoredProcedure;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@date", vMaster.date);
                            command.Parameters.AddWithValue("@company", vMaster.company);
                            command.Parameters.AddWithValue("@divisionCode", divisionCode);
                            command.Parameters.AddWithValue("@estate", vMaster.estate);
                            command.Parameters.AddWithValue("@division", data.division);
                            command.Parameters.AddWithValue("@description", data.description);
                            command.Parameters.AddWithValue("@voucher_type", data.voucher_type);
                            command.Parameters.Add(
                                new SqlParameter()
                                {
                                    Direction = ParameterDirection.Output,
                                    ParameterName = "@MasterId",
                                    SqlDbType = SqlDbType.BigInt
                                }
                            );
                            if (command.ExecuteNonQuery() > 0)
                            {
                                result = 1;
                                long MasterId = Convert.ToInt64(command.Parameters["@MasterId"].Value);
                                command.Dispose();
                                if (MasterId > 0)
                                {
                                    foreach (VoucherDetail vDetail in data.data!)
                                    {
                                        command = new SqlCommand("Voucher.SaveVoucherData", connection);
                                        command.CommandType = System.Data.CommandType.StoredProcedure;
                                        command.Parameters.Clear();
                                        command.Parameters.AddWithValue("@MasterId", MasterId);
                                        command.Parameters.AddWithValue("@account_code", vDetail.account_code);
                                        command.Parameters.AddWithValue("@head_name", vDetail.head_name);
                                        command.Parameters.AddWithValue("@description", vDetail.description);
                                        command.Parameters.AddWithValue("@amount", vDetail.amount);
                                        command.Parameters.Add(
                                            new SqlParameter()
                                            {
                                                Direction = ParameterDirection.Output,
                                                ParameterName = "@DataId",
                                                SqlDbType = SqlDbType.BigInt
                                            }
                                        );
                                        if (command.ExecuteNonQuery() > 0)
                                        {
                                            result = 1;
                                            long DataId = Convert.ToInt64(command.Parameters["@DataId"].Value);
                                            command.Dispose();
                                            if (DataId > 0)
                                            {
                                                foreach (VoucherDetail datas in vDetail.data!)
                                                {
                                                    command = new SqlCommand("Voucher.SaveVoucherDetails", connection);
                                                    command.CommandType = System.Data.CommandType.StoredProcedure;
                                                    command.Parameters.Clear();
                                                    command.Parameters.AddWithValue("@DataId", DataId);
                                                    command.Parameters.AddWithValue("@account_code", datas.account_code);
                                                    command.Parameters.AddWithValue("@head_name", datas.head_name);
                                                    command.Parameters.AddWithValue("@description", datas.description);
                                                    command.Parameters.AddWithValue("@amount", datas.amount);
                                                    if (command.ExecuteNonQuery() > 0)
                                                    {
                                                        result = 1;
                                                    }
                                                    else
                                                    {
                                                        result = 0;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            result = 0;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                result = 0;
                            }
                        }
                        else
                        {
                            result = 2;
                        }
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
                connection.Dispose();
            }
            return result;
        }
        public int SaveFraGetVoucherErrorLog(List<TempErrorLog> errorLog)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                if (errorLog.Count > 0)
                {
                    foreach (TempErrorLog log in errorLog)
                    {
                        SqlCommand command = new("Voucher.SaveFraGetVoucherErrorLog", connection)
                        {
                            CommandType = System.Data.CommandType.StoredProcedure
                        };
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@LogId", log.LogId);
                        command.Parameters.AddWithValue("@GlCode", log.GlCode);
                        command.Parameters.AddWithValue("@GlDescription", log.GlDescription);
                        command.Parameters.AddWithValue("@AccCode", log.AccCode);
                        command.Parameters.AddWithValue("@AccDescription", log.AccDescription);
                        command.Parameters.AddWithValue("@Amount", log.Amount);
                        command.Parameters.AddWithValue("@VoucherDate", log.VoucherDate);
                        command.Parameters.AddWithValue("@Estate", log.Estate);
                        command.Parameters.AddWithValue("@DivisionCode", log.DivisionCode);
                        command.Parameters.AddWithValue("@VoucherType", log.VoucherType);
                        command.Parameters.AddWithValue("@EntryBy", log.EntryBy);
                        if (command.ExecuteNonQuery() > 0)
                        {
                            result = 1;
                        }
                        else
                        {
                            result = 0;
                        }
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
                connection.Dispose();
            }
            return result;
        }
        public List<VoucherDetail> GetVoucher(long MasterId)
        {
            var list = new List<VoucherDetail>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.GetVoucher", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@MasterId", MasterId);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        VoucherDetail model = new VoucherDetail();
                        model.account_code = reader["AccountCode"].ToString() ?? "";
                        model.head_name = reader["AccountHead"].ToString() ?? "";
                        model.description = reader["DataDescription"].ToString() ?? "";
                        model.amount = reader["Amount"].ToString() ?? "";
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
            return list;
        }
        public List<VMasterViewModel> GetVoucherMaster(string divisionCode)
        {
            var list = new List<VMasterViewModel>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.GetVoucherMaster", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@FraDivisionCode", divisionCode);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        VMasterViewModel model = new VMasterViewModel();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.date = reader["VoucherDate"].ToString() ?? "";
                        model.company = reader["CompanyName"].ToString() ?? "";
                        model.division = reader["DivisionName"].ToString() ?? "";
                        model.estate = reader["EstateName"].ToString() ?? "";
                        model.description = reader["VoucherDescription"].ToString() ?? "";
                        model.voucher_type = reader["VoucherType"].ToString() ?? "";
                        model.IsSent = Convert.ToInt32(reader["IsSent"]);
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
            return list;
        }
        public int CheckIfExistVoucher(string? date, string? estate, string? division, string? type)
        {
            int result = 0;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.CheckIfExistVoucher", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@date", date);
                command.Parameters.AddWithValue("@estate", estate);
                command.Parameters.AddWithValue("@division", division);
                command.Parameters.AddWithValue("@type", type);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = Convert.ToInt32(reader["IsExist"]);
                }
                reader.Close();
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
            return result;
        }
    }
}
