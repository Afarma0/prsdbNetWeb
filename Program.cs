using Microsoft.EntityFrameworkCore;
using prsdbNetWeb.Models;

namespace prsdbNetWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.ReferenceHandler =
                  System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddDbContext<prsdbContext>(
        // lambda
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("prsdbConnectionString"))
        );

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}
