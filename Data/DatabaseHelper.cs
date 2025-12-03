using System.Data;
using System.Data.SqlClient;

namespace DocumentManagerApi.Data
{
    public interface IDatabaseHelper
    {
        SqlConnection GetConnection();
        Task<DataTable> ExecuteDataTableAsync(string sql, params SqlParameter[] parameters);
        Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters);
        Task<object?> ExecuteScalarAsync(string sql, params SqlParameter[] parameters);
        Task<int> ExecuteStoredProcedureNonQueryAsync(string spName, params SqlParameter[] parameters);
    }

    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string _connString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connString = _configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connString);
        }

        public async Task<DataTable> ExecuteDataTableAsync(string sql, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters?.Any() == true) cmd.Parameters.AddRange(parameters);
            cmd.CommandType = CommandType.Text;
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            return dt;
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters?.Any() == true) cmd.Parameters.AddRange(parameters);
            cmd.CommandType = CommandType.Text;
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<object?> ExecuteScalarAsync(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters?.Any() == true) cmd.Parameters.AddRange(parameters);
            cmd.CommandType = CommandType.Text;
            await conn.OpenAsync();
            return await cmd.ExecuteScalarAsync();
        }

        public async Task<int> ExecuteStoredProcedureNonQueryAsync(string spName, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(spName, conn);
            if (parameters?.Any() == true) cmd.Parameters.AddRange(parameters);
            cmd.CommandType = CommandType.StoredProcedure;
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
