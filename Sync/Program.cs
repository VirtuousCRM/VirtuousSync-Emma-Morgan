using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Sync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Sync().GetAwaiter().GetResult();
        }

        private static async Task Sync()
        {
            var apiKey = "REPLACE_WITH_API_KEY_PROVIDED";
            var configuration = new Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);

            string connectionString = @"Server=(localdb)\mssqllocaldb;Database=ContactsDb;Trusted_Connection=True;";

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var hasMore = true;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                do
                {
                    var contacts = await virtuousService.GetContactsAsync(skip, take);
                    skip += take;

                    foreach (var contact in contacts.List)
                    {
                        string sql = @"INSERT INTO AbbreviatedContacts (Name, ContactType, ContactName, Address, Email, Phone)
                               VALUES (@Name, @ContactType, @ContactName, @Address, @Email, @Phone)";

                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@Name", contact.Name ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContactType", contact.ContactType ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@ContactName", contact.ContactName ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Address", contact.Address ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Email", contact.Email ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Phone", contact.Phone ?? (object)DBNull.Value);

                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    hasMore = skip < maxContacts;
                }
                while (hasMore);
            }
        }
    }
}
