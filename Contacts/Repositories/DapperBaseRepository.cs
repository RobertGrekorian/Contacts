using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using Dapper;

namespace Contacts.Repositories
{
    public abstract class DapperBaseRepository
    {
        protected readonly string _connectionString;
        protected readonly ILogger _logger;

        protected DapperBaseRepository(IConfiguration configuration, ILogger logger)
        {
            _connectionString = configuration.GetConnectionString("SQL_Local");
            _logger = logger;
        }

        protected IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        protected async Task<IEnumerable<T>> GetListAsync<T>(string sql)
        {
            try
            {
                using var connection = CreateConnection();
                return await connection.QueryAsync<T>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing GetListAsync: {Sql}", sql);
                return Enumerable.Empty<T>();
            }
        }

        protected async Task<IEnumerable<T>> GetListAsync<T>(string sql, object? parameters)
        {
            try
            {
                using var connection = CreateConnection();
                return await connection.QueryAsync<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing GetListAsync with parameters: {Sql}", sql);
                return Enumerable.Empty<T>();
            }
        }

        protected async Task<T?> GetSingleAsync<T>(string sql, object? parameters)
        {
            try
            {
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing GetSingleAsync: {Sql}", sql);
                return default;
            }
        }

        protected async Task<int> ExecuteAsync(string sql, object? parameters)
        {
            try
            {
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing ExecuteAsync: {Sql}", sql);
                return 0;
            }
        }

        protected async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters)
        {
            try
            {
                using var connection = CreateConnection();
                return await connection.ExecuteScalarAsync<T>(sql, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing ExecuteScalarAsync: {Sql}", sql);
                throw;
            }
        }
    }
}
