using dDataAccess;
using Macalms.Model;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace Macalms.Biz
{
    public class SharedBiz
    {
        private readonly IConfiguration _configuration;
        string? connectionString = "";
        public SharedBiz(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        private readonly DbAccess access = new DbAccess();
        public async Task<List<Department>> GetDepartments()
        {
            List<Department> list = new List<Department>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetDepartments", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Department model = new Department();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.DepartmentName = reader["DepartmentName"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.EntryBy = reader["EntryBy"].ToString();
                        model.EntryDate = reader["EntryDate"].ToString();
                        model.ModifyBy = reader["ModifyBy"].ToString();
                        model.ModifyDate = reader["ModifyDate"].ToString();

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
        public async Task<List<Designation>> GetDesignations()
        {
            List<Designation> list = new List<Designation>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetDesignation", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Designation model = new Designation();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.DesignationName = reader["DesignationName"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.EntryBy = reader["EntryBy"].ToString();
                        model.EntryDate = reader["EntryDate"].ToString();
                        model.ModifyBy = reader["ModifyBy"].ToString();
                        model.ModifyDate = reader["ModifyDate"].ToString();

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
        public async Task<List<WorkLocation>> GetWorkLocations()
        {
            List<WorkLocation> list = new List<WorkLocation>();
            SqlDataReader? reader = null;
            SqlConnection connection = access.GetConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand("Macalms.GetWorkLocation", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Clear();
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        WorkLocation model = new WorkLocation();
                        model.RecordId = Convert.ToInt64(reader["RecordId"]);
                        model.LocationName = reader["LocationName"].ToString();
                        model.LocationTag = reader["LocationTag"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.EntryBy = reader["EntryBy"].ToString();
                        model.EntryDate = reader["EntryDate"].ToString();
                        model.ModifyBy = reader["ModifyBy"].ToString();
                        model.ModifyDate = reader["ModifyDate"].ToString();

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
