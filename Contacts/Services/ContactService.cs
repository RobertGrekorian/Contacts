using ContactsData.Models;
using ContactsData.Models.Dto;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using X.PagedList;
using X.PagedList.Extensions;

namespace Contacts.Services
{
    public interface IContactService
    {
        Task<IPagedList<ContactDto>> GetFilteredContactsAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = "",
            string sortColumn = "FirstName",
            string sortDirection = "ASC"
        );
        Task<IPagedList<ContactDto>> GetAllFilteredContactsAsync(
        int pageNumber,
        int pageSize,
        string searchTerm = "",
        string sortColumn = "FirstName",
        string sortDirection = "ASC"
        );
    }

    public class ContactService : IContactService
    {
        private readonly string _connectionString;

        public ContactService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQL_Local");
        }

        public async Task<IPagedList<ContactDto>> GetFilteredContactsAsync(
            int pageNumber,
            int pageSize,
            string searchTerm = "",
            string sortColumn = "FirstName",
            string sortDirection = "ASC"
        )
        {
            // Define safe sortable columns (DB column names)
            
            var validSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["ID"] = "ID",
                ["FirstName"] = "FirstName",
                ["LastName"] = "LastName",
                ["Email"] = "Email",
                ["Phone"] = "Phone",
                ["City"] = "City",
                ["State"] = "State",
                ["PostalCode"] = "PostalCode"
            };

            // Validate and fallback
            // if the sortColumn coming from query string is in the dictionary Use it 
            // if not default it to FirstName , prevent SQL Injection (Ignore lowercae or capitalcase letters)
            sortColumn = validSortColumns.TryGetValue(sortColumn, out var dbSortColumn)
                ? dbSortColumn
                : "FirstName";
            // if the sortDirection coming from query string is in the dictionary and is equal DESC Use it 
            // if not toggle it to ASC , prevent SQL Injection (Ignore lowercae or capitalcase letters)
            sortDirection = sortDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                ? "DESC"
                : "ASC";

            var offset = (pageNumber - 1) * pageSize;
            var searchPattern = $"%{searchTerm}%";

            var sql = $@"
                SELECT ID, FirstName, LastName, Email, Address, Phone, City, State, PostalCode
                FROM contacts
                WHERE FirstName Like @Search 
                   OR LastName Like @Search 
                   OR Email Like @Search 
                   OR Address Like @Search 
                   OR Phone Like @Search 
                   OR City Like @Search 
                   OR State Like @Search 
                   OR PostalCode Like @Search
                ORDER BY {dbSortColumn} {sortDirection}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

                SELECT COUNT(*)
                FROM contacts
                WHERE FirstName Like @Search 
                   OR LastName Like @Search 
                   OR Email Like @Search 
                   OR Address Like @Search 
                   OR Phone Like @Search 
                   OR City Like @Search 
                   OR State Like @Search 
                   OR PostalCode Like @Search;
            ";

            var parameters = new
            {
                Search = searchPattern,
                Offset = offset,
                PageSize = pageSize
            };

            //using var connection = new NpgsqlConnection(_connectionString);
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(sql, parameters);

            var items = (await multi.ReadAsync<ContactDto>()).ToList();
            var totalCount = await multi.ReadFirstAsync<int>();

            return new StaticPagedList<ContactDto>(items, pageNumber, pageSize, totalCount);
        }
        //Use GetAllFilteredContactsAsync when the result has limited record counts 
        public async Task<IPagedList<ContactDto>> GetAllFilteredContactsAsync(
        int pageNumber,
        int pageSize,
        string searchTerm = "",
        string sortColumn = "FirstName",
        string sortDirection = "ASC"
        )
        {
            var validSortColumns = new Dictionary<string, Func<ContactDto, object>>(StringComparer.OrdinalIgnoreCase)
            {
                ["ID"] = c => c.Id,
                ["FirstName"] = c => c.FirstName,
                ["LastName"] = c => c.LastName,
                ["Email"] = c => c.Email,
                ["Phone"] = c => c.Phone,
                ["City"] = c => c.City,
                ["State"] = c => c.State,
                ["PostalCode"] = c => c.PostalCode,
                ["CountryName"] = c => c.CountryName
            };

            Func<ContactDto, object> sortExpression = validSortColumns.TryGetValue(sortColumn, out var column)
                ? column
                : c => c.FirstName;

            var searchPattern = $"%{searchTerm}%";

            var sql = @"
                        SELECT ID, FirstName, LastName, Email, Address, Phone, City, State, PostalCode,CountryId
                        FROM Contacts
                        WHERE FirstName LIKE @Search 
                           OR LastName LIKE @Search 
                           OR Email LIKE @Search 
                           OR Address LIKE @Search 
                           OR Phone LIKE @Search 
                           OR City LIKE @Search 
                           OR State LIKE @Search 
                           OR PostalCode LIKE @Search;
                    ";

            var parameters = new { Search = searchPattern };

            using var connection = new SqlConnection(_connectionString);
            var allContacts = (await connection.QueryAsync<ContactDto>(sql, parameters)).ToList();

            string countrySql = @"
                                SELECT CountryId, CountryName 
                                FROM Countries";
            var countries = (await connection.QueryAsync<(int CountryId,String CountryName)>(countrySql)).ToList();

            var enrichedContacts = (from c in allContacts
                                    join country in countries on c.CountryId equals country.CountryId 
                                   // into gj from sub in gj.DefaultIfEmpty()
                                    select new ContactDto
                                    {
                                        Id = c.Id,
                                        FirstName = c.FirstName,
                                        LastName = c.LastName,
                                        Email = c.Email,
                                        Address = c.Address,
                                        Phone = c.Phone,
                                        City = c.City,
                                        State = c.State,
                                        PostalCode = c.PostalCode,
                                        CountryId = c.CountryId,
                                        CountryName = country.CountryName ?? "Unknown"
                                    }).ToList();

            // Apply sorting
            var sorted = sortDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                ? enrichedContacts.OrderByDescending(sortExpression)
                : enrichedContacts.OrderBy(sortExpression);

            // Return PagedList using LINQ
            return sorted.ToPagedList(pageNumber, pageSize);
        }
    }
}
