using ContactsData.Models;
using ContactsData.Models.Dto;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;
using X.PagedList;

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
            sortColumn = validSortColumns.TryGetValue(sortColumn, out var dbSortColumn)
                ? dbSortColumn
                : "FirstName";

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
    }
}

//SQL version
//using ContactsData.Models;
//using Dapper;
//using Microsoft.Data.SqlClient;
//using X.PagedList;

//namespace Contacts.Services
//{
//    public interface IContactService
//    {
//        Task<IPagedList<Contact>> GetFilteredContactsAsync(
//            int pageNumber,
//            int pageSize,
//            string searchTerm = "",
//            string sortColumn = "FirstName",
//            string sortDirection = "ASC"
//        );
//    }

//    public class ContactService : IContactService
//    {
//        private readonly string _connectionString;

//        public ContactService(IConfiguration configuration)
//        {
//            _connectionString = configuration.GetConnectionString("SQL_Local");
//        }

//        public async Task<IPagedList<Contact>> GetFilteredContactsAsync(
//            int pageNumber,
//            int pageSize,
//            string searchTerm = "",
//            string sortColumn = "FirstName",
//            string sortDirection = "ASC"
//        )
//        {
//            // Define valid sortable columns (SQL Server column names)
//            var validSortColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
//            {
//                ["ID"] = "ID",
//                ["FirstName"] = "FirstName",
//                ["LastName"] = "LastName",
//                ["Email"] = "Email",
//                ["Phone"] = "Phone",
//                ["City"] = "City",
//                ["State"] = "State",
//                ["PostalCode"] = "PostalCode"
//            };

//            sortColumn = validSortColumns.TryGetValue(sortColumn, out var dbSortColumn)
//                ? dbSortColumn
//                : "FirstName";

//            sortDirection = sortDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase)
//                ? "DESC"
//                : "ASC";

//            var offset = (pageNumber - 1) * pageSize;
//            var searchPattern = $"%{searchTerm}%";

//            var sql = $@"
//                SELECT ID, FirstName, LastName, Email, Address, Phone, City, State, PostalCode
//                FROM Contacts
//                WHERE FirstName LIKE @Search 
//                   OR LastName LIKE @Search 
//                   OR Email LIKE @Search 
//                   OR Address LIKE @Search 
//                   OR Phone LIKE @Search 
//                   OR City LIKE @Search 
//                   OR State LIKE @Search 
//                   OR PostalCode LIKE @Search
//                ORDER BY {dbSortColumn} {sortDirection}
//                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

//                SELECT COUNT(*)
//                FROM Contacts
//                WHERE FirstName LIKE @Search 
//                   OR LastName LIKE @Search 
//                   OR Email LIKE @Search 
//                   OR Address LIKE @Search 
//                   OR Phone LIKE @Search 
//                   OR City LIKE @Search 
//                   OR State LIKE @Search 
//                   OR PostalCode LIKE @Search;
//            ";

//            var parameters = new
//            {
//                Search = searchPattern,
//                Offset = offset,
//                PageSize = pageSize
//            };

//            using var connection = new SqlConnection(_connectionString);
//            await connection.OpenAsync();

//            using var multi = await connection.QueryMultipleAsync(sql, parameters);

//            var items = (await multi.ReadAsync<Contact>()).ToList();
//            var totalCount = await multi.ReadFirstAsync<int>();

//            return new StaticPagedList<Contact>(items, pageNumber, pageSize, totalCount);
//        }
//    }
//}