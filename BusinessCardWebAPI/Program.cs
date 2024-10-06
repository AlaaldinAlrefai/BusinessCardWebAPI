
using BusinessCardWebAPI.Configrations;
using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.IReposetory;
using BusinessCardWebAPI.Core.IServieces;
using BusinessCardWebAPI.Infra.Reposetory;
using BusinessCardWebAPI.Infra.Servieces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("BusinessCardDbConnectionString");

            if (connectionString == null)
            {
                Console.WriteLine("Connection string not found.");
            }
            else
            {
                using (var connection = new SqlConnection(connectionString))
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

            builder.Services.AddDbContext<BusinessCardDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(MapperConfig));
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IBusinessCardsReposetory, BusinessCardsReposetory>();
            builder.Services.AddScoped<IBusinessCardsServieces, BusinessCardsServieces>();

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