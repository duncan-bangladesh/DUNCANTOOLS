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
        public int SaveVoucherMaster(VoucherMaster master, string divisionCode)
        {
            int result = 0;
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    if (master != null)
                    {
                        VoucherMaster vMaster = master;
                        foreach (VoucherData data in vMaster.data!)
                        {
                            if(CheckIfExistVoucher(vMaster.date, vMaster.estate, data.division, data.voucher_type) == 0)
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
                                if(command.ExecuteNonQuery() > 0)
                                {
                                    result = 1;
                                    long MasterId = Convert.ToInt64(command.Parameters["@MasterId"].Value);
                                    command.Dispose();
                                    if (MasterId > 0)
                                    {
                                        //var vAmount = data.data!.Sum(x=> Convert.ToDouble(x.amount));
                                        //VoucherDetail voucherDetail = new VoucherDetail()
                                        //{
                                        //    account_code = "B5AA02AA",
                                        //    head_name = "PROVISION FOR LABOUR WAGES",
                                        //    amount = vAmount.ToString(),
                                        //    description = data.data!.First().description
                                        //};
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
                                                        if(command.ExecuteNonQuery() > 0)
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
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
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
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
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
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
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
