
using Microsoft.Data.SqlClient;

namespace BusinessCardWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var ConnectionString = builder.Configuration.GetConnectionString("BusinessCardDbConnectionString");
            if (ConnectionString == null)
            {
                Console.WriteLine("Connection string not found.");
            }
            else
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine("Connection opened successfully.");
                        connection.Close();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while trying to open the connection: " + ex.Message);
                    }
                }
            }


        // Add services to the container.

        builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}