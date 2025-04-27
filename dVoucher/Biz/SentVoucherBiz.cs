using dDataAccess;
using dVoucher.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace dVoucher.Biz
{
    public class SentVoucherBiz
    {
        private readonly IConfiguration _configuration;
        private string? amteConnectionString;
        private string? connectionString;
        public SentVoucherBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        private int GetBatchSerialNo(string ConString)
        {
            int result = 0;
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(ConString);
            try
            {
                SqlCommand command = new("Pr_GetBatchSerialNo", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();                
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = Convert.ToInt32(reader["SerialNo"]);
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
            return result;
        }
        private int GetVoucherSerialNo(string ConString)
        {
            int result = 0;
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(ConString);
            try
            {
                SqlCommand command = new("Pr_GetVoucherSerialNo", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result = Convert.ToInt32(reader["SerialNo"]);
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
            return result;
        }
        private List<SentVoucherMaster> GetSentVouchers(long MasterId, string FraDivisionCode,string? EstateConString)
        {
            var list = new List<SentVoucherMaster>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                int BatchSerialNo = GetBatchSerialNo(EstateConString!), VoucherSerialNo = GetVoucherSerialNo(EstateConString!);
                SqlCommand command = new("Voucher.GetSentVoucherByMasterId", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@MasterId", MasterId);
                command.Parameters.AddWithValue("@FraDivisionCode", FraDivisionCode);
                command.Parameters.AddWithValue("@BatchSerialNo", BatchSerialNo);
                command.Parameters.AddWithValue("@VoucherSerialNo", VoucherSerialNo);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SentVoucherMaster model = new SentVoucherMaster();
                        model.TranDate = reader["TranDate"].ToString() ?? "";
                        model.TranType = Convert.ToInt32(reader["TranType"]);
                        model.Narration = reader["Narration"].ToString() ?? "";
                        model.CompanyCode = reader["CompanyCode"].ToString() ?? "";
                        model.EstateCode = reader["EstateCode"].ToString() ?? "";
                        model.SerialNo = Convert.ToInt32(reader["SerialNo"]);
                        model.BatchNo = reader["BatchNo"].ToString() ?? "";
                        model.VoucherSerialNo = Convert.ToInt32(reader["VoucherSerialNo"]);
                        model.VoucherNo = reader["VoucherNo"].ToString() ?? "";
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
        private List<VoucherDetailModel> GetSentVoucherDataByMasterId(long MasterId)
        {
            var list = new List<VoucherDetailModel>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.GetSentVoucherDataByMasterId", connection)
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
                        VoucherDetailModel model = new VoucherDetailModel();
                        model.id = Convert.ToInt32(reader["RecordId"]);
                        model.account_code = reader["AccountCode"].ToString() ?? "";
                        model.head_name = reader["AccountHead"].ToString() ?? "";
                        model.description = reader["DataDescription"].ToString() ?? "";
                        model.amount = reader["Amount"].ToString() ?? "";
                        model.tranSide = 1;
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
        private List<VoucherDetail> GetSentVoucherDetailsByVoucherDataId(int DataId)
        {
            var list = new List<VoucherDetail>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.GetSentVoucherDetailsByVoucherDataId", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@DataId", DataId);
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
        private int GetGlIdByCodeName(string CodeName, string conString)
        {
            int glId = 0;
            SqlDataReader? reader = null;            
            SqlConnection connection = access.GetConnection(conString);
            try
            {
                SqlCommand command = new("GetGLCodeByCodeName", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@CodeName", CodeName);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        glId = Convert.ToInt32(reader["GLID"]);
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
            return glId;
        }
        private List<TaskCodeRef> GetTaskCodeRef(string Code, string? conString)
        {
            var list = new List<TaskCodeRef>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(conString);
            try
            {
                SqlCommand command = new("GetTaskCodeIdByCodeName", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@CodeName", Code);
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TaskCodeRef model = new TaskCodeRef();
                        model.RecordID = Convert.ToInt32(reader["RecordID"]);
                        model.ObjectID = Convert.ToInt32(reader["ObjectID"]);
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
        private bool UpdateVoucherMasterIsSentStatus(long masterId)
        {
            bool result = false;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new("Voucher.UpdateVoucherMasterIsSentStatus", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@MasterId", masterId);
                if (command.ExecuteNonQuery() > 0)
                {
                    result = true;
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

        //Try to impliment SQL Transaction here for multiple SQL operations
        public int VoucherSentToCharms(long MasterId, string FraDivisionCode, string? conString)
        {
            int result = 0;
            string cString = _configuration.GetConnectionString(conString!)!;
            SqlConnection connection = access.GetConnection(cString);
            var master = GetSentVouchers(MasterId, FraDivisionCode, cString);
            if (master != null)
            {
                try
                {
                    SqlCommand command = new SqlCommand();
                    foreach (var voucher in master)
                    {
                        command = new SqlCommand("InsertTranMaster_FraTool_Confirmation", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@TranDate", voucher.TranDate);
                        command.Parameters.AddWithValue("@TranType", voucher.TranType);
                        command.Parameters.AddWithValue("@BatchNo", voucher.BatchNo);
                        command.Parameters.AddWithValue("@SerialNo", voucher.SerialNo);
                        command.Parameters.AddWithValue("@VoucherNo", voucher.VoucherNo);
                        command.Parameters.AddWithValue("@Narration", voucher.Narration);
                        command.Parameters.AddWithValue("@EntryUserID", 9999);
                        command.Parameters.AddWithValue("@VoucherSerialNo", voucher.VoucherSerialNo);
                        command.Parameters.Add(
                            new SqlParameter()
                            {
                                Direction = ParameterDirection.Output,
                                ParameterName = "@TranId",
                                SqlDbType = SqlDbType.BigInt
                            }
                        );
                        if (command.ExecuteNonQuery() > 0)
                        {
                            result = 1;
                            long TranId = Convert.ToInt64(command.Parameters["@TranId"].Value);
                            command.Dispose();
                            if (TranId > 0)
                            {
                                //Load Voucher Data based on MasterId
                                var voucherData = GetSentVoucherDataByMasterId(MasterId);
                                double vAmount = voucherData.Sum(x => Convert.ToDouble(x.amount));
                                var desc = voucherData.First().description;
                                VoucherDetailModel voucherDetailModel = new VoucherDetailModel()
                                {
                                    account_code = "B5AA02AA",
                                    head_name = "PROVISION FOR LABOUR WAGES",
                                    amount = vAmount.ToString(),
                                    description = voucherData.First().description,
                                    id = 0,
                                    tranSide = 2
                                };
                                voucherData.Add(voucherDetailModel);
                                foreach (var data in voucherData)
                                {
                                    int glId = GetGlIdByCodeName(data.account_code!, cString!);
                                    command = new SqlCommand("InsertTranDetail_FraTool_Confirmation", connection);
                                    command.CommandType = System.Data.CommandType.StoredProcedure;
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@TranId", TranId);
                                    command.Parameters.AddWithValue("@GlId", glId);
                                    command.Parameters.AddWithValue("@TranSide", data.tranSide);
                                    command.Parameters.AddWithValue("@Amount", data.amount);
                                    command.Parameters.AddWithValue("@Description", data.description);
                                    command.Parameters.Add(
                                        new SqlParameter()
                                        {
                                            Direction = ParameterDirection.Output,
                                            ParameterName = "@TranDetailId",
                                            SqlDbType = SqlDbType.BigInt
                                        }
                                    );
                                    if (command.ExecuteNonQuery() > 0)
                                    {
                                        result = 2;
                                        int detailId = Convert.ToInt32(command.Parameters["@TranDetailId"].Value);
                                        command.Dispose();
                                        if (detailId > 0)
                                        {
                                            //Load and insert Transection details to TranUserRecord
                                            List<VoucherDetail> voucherDetails = new List<VoucherDetail>();
                                            if (data.id != 0)
                                            {
                                                voucherDetails = GetSentVoucherDetailsByVoucherDataId(data.id);
                                            }
                                            else
                                            {
                                                VoucherDetail vDetail = new VoucherDetail()
                                                {
                                                    account_code = "LW01",
                                                    head_name = "Wages Provision- Garden",
                                                    amount = vAmount.ToString(),
                                                    description = "FRA > Total Provision"
                                                };
                                                voucherDetails.Add(vDetail);
                                            }
                                            foreach (var details in voucherDetails)
                                            {
                                                if (data.account_code != details.account_code)
                                                {
                                                    var taskRef = GetTaskCodeRef(details.account_code!, cString!);
                                                    if(taskRef.Count > 0)
                                                    {
                                                        command = new SqlCommand("InsertTranUserRecord_FraTool_Confirmation", connection);
                                                        command.CommandType = System.Data.CommandType.StoredProcedure;
                                                        command.Parameters.Clear();
                                                        command.Parameters.AddWithValue("@TranId", TranId);
                                                        command.Parameters.AddWithValue("@DetailId", detailId);
                                                        command.Parameters.AddWithValue("@GlId", glId);
                                                        command.Parameters.AddWithValue("@UooneId", taskRef.First().ObjectID);
                                                        command.Parameters.AddWithValue("@UroneId", taskRef.First().RecordID);
                                                        command.Parameters.AddWithValue("@Amount", details.amount);
                                                        command.Parameters.AddWithValue("@Description", details.description);
                                                        if (command.ExecuteNonQuery() > 0)
                                                        {
                                                            result = 3;
                                                        }
                                                    }                                                    
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        connection.Close();
                    }
                    connection.Dispose();
                }
            }
            if (result == 3)
            {
                //Update IsSent to 1 by masterId
                bool final = UpdateVoucherMasterIsSentStatus(MasterId);
                if (final == true)
                {
                    result = 100;
                }
            }
            return result;
        }

        //Previously run perfectly ok but SQL Transection wasn't available
        //public int VoucherSentToCharms(long MasterId, string FraDivisionCode, string? conString)
        //{
        //    int result = 0;
        //    string cString = _configuration.GetConnectionString(conString!)!;
        //    SqlConnection connection = access.GetConnection(cString);
        //    try
        //    {
        //        var master = GetSentVouchers(MasterId, FraDivisionCode,cString);
        //        if (master != null) {
        //            foreach (var voucher in master) 
        //            {
        //                SqlCommand command = new("InsertTranMaster_FraTool_Confirmation", connection)
        //                {
        //                    CommandType = System.Data.CommandType.StoredProcedure
        //                };
        //                command.Parameters.Clear();
        //                command.Parameters.AddWithValue("@TranDate", voucher.TranDate);
        //                command.Parameters.AddWithValue("@TranType", voucher.TranType);
        //                command.Parameters.AddWithValue("@BatchNo", voucher.BatchNo);
        //                command.Parameters.AddWithValue("@SerialNo", voucher.SerialNo);
        //                command.Parameters.AddWithValue("@VoucherNo", voucher.VoucherNo);
        //                command.Parameters.AddWithValue("@Narration", voucher.Narration);
        //                command.Parameters.AddWithValue("@EntryUserID", 9999);
        //                command.Parameters.AddWithValue("@VoucherSerialNo", voucher.VoucherSerialNo);
        //                command.Parameters.Add(
        //                    new SqlParameter()
        //                    {
        //                        Direction = ParameterDirection.Output,
        //                        ParameterName = "@TranId",
        //                        SqlDbType = SqlDbType.BigInt
        //                    }
        //                );
        //                if (command.ExecuteNonQuery() > 0)
        //                {
        //                    result = 1;
        //                    long TranId = Convert.ToInt64(command.Parameters["@TranId"].Value);
        //                    command.Dispose();
        //                    if (TranId > 0)
        //                    {
        //                        //Load Voucher Data based on MasterId
        //                        var voucherData = GetSentVoucherDataByMasterId(MasterId);
        //                        double vAmount = voucherData.Sum(x => Convert.ToDouble(x.amount));
        //                        var desc = voucherData.First().description;
        //                        VoucherDetailModel voucherDetailModel = new VoucherDetailModel()
        //                        {
        //                            account_code = "B5AA02AA",
        //                            head_name = "PROVISION FOR LABOUR WAGES",
        //                            amount = vAmount.ToString(),
        //                            description = voucherData.First().description,
        //                            id = 0,
        //                            tranSide = 2
        //                        };
        //                        voucherData.Add(voucherDetailModel);
        //                        foreach (var data in voucherData)
        //                        {
        //                            int glId = GetGlIdByCodeName(data.account_code!, cString!);
        //                            command = new SqlCommand("InsertTranDetail_FraTool_Confirmation", connection);
        //                            command.CommandType = System.Data.CommandType.StoredProcedure;
        //                            command.Parameters.Clear();
        //                            command.Parameters.AddWithValue("@TranId", TranId);
        //                            command.Parameters.AddWithValue("@GlId", glId);
        //                            command.Parameters.AddWithValue("@TranSide", data.tranSide);
        //                            command.Parameters.AddWithValue("@Amount", data.amount);
        //                            command.Parameters.AddWithValue("@Description", data.description);
        //                            command.Parameters.Add(
        //                                new SqlParameter()
        //                                {
        //                                    Direction = ParameterDirection.Output,
        //                                    ParameterName = "@TranDetailId",
        //                                    SqlDbType = SqlDbType.BigInt
        //                                }
        //                            );
        //                            if (command.ExecuteNonQuery() > 0)
        //                            {
        //                                result = 2;
        //                                int detailId = Convert.ToInt32(command.Parameters["@TranDetailId"].Value);
        //                                command.Dispose();
        //                                if (detailId > 0)
        //                                {
        //                                    //Load and insert Transection details to TranUserRecord
        //                                    List<VoucherDetail> voucherDetails = new List<VoucherDetail>();
        //                                    if (data.id != 0)
        //                                    {
        //                                        voucherDetails = GetSentVoucherDetailsByVoucherDataId(data.id);
        //                                    }
        //                                    else
        //                                    {
        //                                        VoucherDetail vDetail = new VoucherDetail()
        //                                        {
        //                                            account_code = "LW01",
        //                                            head_name = "Wages Provision- Garden",
        //                                            amount = vAmount.ToString(),
        //                                            description = "FRA > Total Provision"
        //                                        };
        //                                        voucherDetails.Add(vDetail);
        //                                    }
        //                                    foreach (var details in voucherDetails)
        //                                    {
        //                                        var taskRef = GetTaskCodeRef(details.account_code!, cString!);
        //                                        command = new SqlCommand("InsertTranUserRecord_FraTool_Confirmation", connection);
        //                                        command.CommandType = System.Data.CommandType.StoredProcedure;
        //                                        command.Parameters.Clear();
        //                                        command.Parameters.AddWithValue("@TranId", TranId);
        //                                        command.Parameters.AddWithValue("@DetailId", detailId);
        //                                        command.Parameters.AddWithValue("@GlId", glId);
        //                                        command.Parameters.AddWithValue("@UooneId", taskRef.First().ObjectID);
        //                                        command.Parameters.AddWithValue("@UroneId", taskRef.First().RecordID);
        //                                        command.Parameters.AddWithValue("@Amount", details.amount);
        //                                        command.Parameters.AddWithValue("@Description", details.description);
        //                                        if (command.ExecuteNonQuery() > 0)
        //                                        {
        //                                            result = 3;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //                connection.Close();
        //            }
        //        }
        //        if(result == 3)
        //        {
        //            //Update IsSent to 1 by masterId
        //            bool final = UpdateVoucherMasterIsSentStatus(MasterId);
        //            if (final == true)
        //            {
        //                result = 100;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        if (connection.State != ConnectionState.Closed)
        //        {
        //            connection.Close();
        //        }
        //        connection.Dispose ();
        //    }
        //    return result;
        //}
    }
}
