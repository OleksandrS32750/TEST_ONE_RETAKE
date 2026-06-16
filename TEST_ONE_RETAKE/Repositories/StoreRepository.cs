using Microsoft.Data.SqlClient;
using System.Diagnostics.Metrics;
using TEST_ONE_RETAKE.DTOs;

namespace TEST_ONE_RETAKE.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly string _connectionString;

        public StoreRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public async Task<STORE_DTO> GetAllStoresAsync(string? name)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            STORE_DTO? store_dto = null;

            var stores = new Dictionary<int, Store>();

            const string sql = @"
            SELECT s.Code,s.Name,b.Id,b.Title, b.Synposis, b.BasePrice,g.Name,a.Name
            FROM Stores s 
            LEFT JOIN StoreBooks sb on sb.StoreCode = s.Code
            LEFT JOIN Books b on b.Id = sb.BookId
            JOIN Genres g on g.Id = b.GenreId
            JOIN Authors a on a.Id = b.AuthorId
            WHERE (@Name IS NULL OR Name LIKE '%' + @Name + '%')";



            await using (var command = new SqlCommand(sql,connection))
            {
                command.Parameters.AddWithValue("@Name", name);


                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var store = new Store
                    {
                        Code = reader.GetString(reader.GetOrdinal("Code")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))
                    };

                    if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                    {
                        var book = new Book
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Synopsis = reader.IsDBNull(reader.GetOrdinal("Synposis")) ? null : reader.GetString(reader.GetOrdinal("Synposis")),
                            BasePrice = reader.GetDecimal(reader.GetOrdinal("BasePrice")),
                            Genre = new Genre { Name = reader.GetString(reader.GetOrdinal("Name")) },
                            Author = new Author { Name = reader.GetString(reader.GetOrdinal("Name")) }
                        };
                        store.Books.Add(book);
                    }

                    store_dto.Stores.Add(store);

                }
            }

            return store_dto;
        }


        public async Task<string?> CreateStoreAsync(CreateStoreDTO createStoreDTO)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = connection.BeginTransaction();

    
            string Code;

            await using (var command = new SqlCommand("INSERT INTO Stores (Code,Name) OUTPUT INSERTED.Code VALUES (@Code,@Name)", connection, transaction))
            {
                command.Parameters.AddWithValue("@Code", createStoreDTO.Code);
                command.Parameters.AddWithValue("@Name", createStoreDTO.Name);
                Code = (string)await command.ExecuteScalarAsync();
            }

            if (createStoreDTO.BookDTOs is not null)
            {
                    foreach (var bookDTO in createStoreDTO.BookDTOs)
                    {
                        // to check if book with provided code exists in database
                        int BookId;

                        await using (var bookCommand = new SqlCommand("Select Id FROM Book Where Id = @Id", connection, transaction))
                            {
                                bookCommand.Parameters.AddWithValue("@Id", bookDTO.Id);

                                var existingBookId = await bookCommand.ExecuteScalarAsync();

                                if (existingBookId is not null)
                                {
                                    BookId = (int)existingBookId;
                                } else
                        {
                                return $"Wrong Book Id : {bookDTO.Id}"; // no book in database => cant add it to store,provide errmsg
                        }
                            }

                    await using (var command = new SqlCommand("INSERT INTO StoreBooks (StoreCode,BookId,Stock,SellingPrice) VALUES (@StoreCode,@BookId,@Stock,@SellingPrice)", connection, transaction))
                    {
                        command.Parameters.AddWithValue("@StoreCode", Code);
                        command.Parameters.AddWithValue("@BookId", bookDTO.Id);
                        command.Parameters.AddWithValue("@Stock", bookDTO.Stock);
                        command.Parameters.AddWithValue("@SellingPrice", bookDTO.SellingPrice);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }

            // null,if no errmsgs occured
            return null;
        }

   }
}
