using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Dto.ApiSources.Products;
using SnackTech.Orders.Driver.API.Configuration;
using SnackTech.Orders.Driver.API.Configuration.HealthChecks;
using SnackTech.Orders.Driver.DataBase;
using SnackTech.Orders.Driver.DataBase.Context;
using SnackTech.Orders.Driver.Payments;
using SnackTech.Orders.Driver.Products;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SnackTech.Orders.Driver.API
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            await ConfigureDatabaseAsync(app);

            ConfigureMiddleware(app);

            await app.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PagamentoApiSettings>(configuration.GetSection("PagamentoApiSettings"));
            services.Configure<ProdutoApiSettings>(configuration.GetSection("ProdutoApiSettings"));
            services.AddHttpServices();
            services.AddAdapterPagamentoApi();
            services.AddAdapterProdutoApi();
            services.AddAdapterDatabaseRepositories();
            services.AddDomainControllers();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SnackTech", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            string dbConnectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'DefaultConnection'.");
            }

            services.AddDbContext<RepositoryDbContext>(options =>
                options.UseSqlServer(dbConnectionString));

            services.AddHealthChecks()
                .ConfigureSQLHealthCheck();
        }

        private static async Task ConfigureDatabaseAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SnackTech Order API v1");
            });

            app.UseCustomHealthChecks();
            app.UseAuthorization();

            // Redirecionamento da URL raiz para /swagger
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });

            app.MapControllers();
        }
    }
}