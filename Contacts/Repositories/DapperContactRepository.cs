using ContactsData.Models;
using ContactsData.Models.Dto;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Contacts.Repositories
{
    public interface IDapperContactRepository
    {
        Task<IEnumerable<ContactDto>> GetListAsync();
        Task<Contact?> GetByEmailAsync(string email);
        //Task<Contact?> GetAsync(int id);
        //Task CreateAsync(Contact contact);
        //Task UpdateAsync(Contact contact);
        //Task DeleteAsync(int id);
    }
    public class DapperContactRepository : DapperBaseRepository, IDapperContactRepository
    {
        private readonly string _connectionString;

        public DapperContactRepository(IConfiguration configuration, ILogger<DapperContactRepository> logger)
            : base(configuration, logger)
        {
            
        }

        public async Task<IEnumerable<ContactDto>> GetListAsync()
        {
            try
            {
                using var connect = new SqlConnection(_connectionString);

                return await connect.QueryAsync<ContactDto>("SELECT * FROM Contacts as contactTable inner join countries as countryTable on "+
                                                         "contactTable.countryId = countryTable.CountryId");
                

            }
            catch (Exception e)
            {
                return Enumerable.Empty<ContactDto>();
                throw;
            }
        }

        public async Task<IEnumerable<ContactDto>> GetListAsyncIDGreateThan(int id)
        {
            var sql = @"SELECT * FROM Contacts AS contactTable 
                INNER JOIN countries AS countryTable 
                ON contactTable.countryId = countryTable.CountryId 
                WHERE contactTable.countryId > @CountryId";
             sql = @"
                    SELECT 
                        contactTable.Id,
                        contactTable.Name,
                        contactTable.Email,
                        countryTable.Name AS CountryName
                    FROM Contacts AS contactTable 
                    INNER JOIN countries AS countryTable 
                        ON contactTable.countryId = countryTable.CountryId 
                    WHERE contactTable.countryId > @CountryId";

            var k = await GetListAsync<ContactDto>(sql, new { CountryId = id });

            return k.ToList();
        }
        public async Task<Contact?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Contacts WHERE Id = @Id";
            return await GetSingleAsync<Contact>(sql, new { Id = id });
        }
        public async Task<Contact?> GetByEmailAsync(string email)
        {
            var sql = "SELECT * FROM Contacts WHERE Email = @Email";
            return await GetSingleAsync<Contact>(sql, new { Email = email });
        }
        public async Task<int> CreateAsync(Contact contact)
        {
            var sql = @"INSERT INTO Contacts (Name, Price, Stock, CreatedDate) 
                VALUES (@Name, @Price, @Stock, @CreatedDate);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await ExecuteScalarAsync<int>(sql, contact);

        }

        public async Task<bool> UpdateAsync(Contact contact)
        {
            var sql = @"UPDATE Contacts 
                SET Name = @Name, Price = @Price, Stock = @Stock 
                WHERE Id = @Id";

            var rows = await ExecuteAsync(sql, contact);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "DELETE FROM Contacts WHERE Id = @Id";
            var rows = await ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }
}
