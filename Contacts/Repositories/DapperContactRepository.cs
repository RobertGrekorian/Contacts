using Contacts.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Contacts.Repositories
{
    public interface IDapperContactRepository
    {
        Task<IEnumerable<Contact>> GetListAsync();
        //Task<Contact?> GetAsync(int id);
        //Task CreateAsync(Contact contact);
        //Task UpdateAsync(Contact contact);
        //Task DeleteAsync(int id);
    }
    public class DapperContactRepository : IDapperContactRepository
    {
        private readonly string _connectionString;

        public DapperContactRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQL_Local");
        }

        public async Task<IEnumerable<Contact>> GetListAsync()
        {
            try
            {
                using var connect = new SqlConnection(_connectionString);

                return await connect.QueryAsync<Contact>("SELECT * FROM Contacts");

            }
            catch (Exception e)
            {
                return Enumerable.Empty<Contact>();
                throw;
            }
        }

        public async Task<Contact> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Contact>(
                "SELECT * FROM Contacts WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CreateAsync(Contact contact)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Contacts (Name, Price, Stock, CreatedDate) 
                    VALUES (@Name, @Price, @Stock, @CreatedDate);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

            return await connection.ExecuteScalarAsync<int>(sql, contact);
        }

        public async Task<bool> UpdateAsync(Contact contact)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"UPDATE Contacts 
                   SET Name = @Name, Price = @Price, Stock = @Stock 
                   WHERE Id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, contact);
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync(
                "DELETE FROM Contacts WHERE Id = @Id", new { Id = id });
            return affectedRows > 0;
        }
    }
}
