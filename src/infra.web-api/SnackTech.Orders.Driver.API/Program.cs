using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SnackTech.Orders.Driver.API.Configuration;
using SnackTech.Orders.Driver.API.Configuration.HealthChecks;
using SnackTech.Orders.Driver.DataBase;
using SnackTech.Orders.Driver.DataBase.Context;
using System.Reflection;
using SnackTech.Orders.Driver.Products;
using SnackTech.Orders.Driver.Payments;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Dto.ApiSources.Products;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<PagamentoApiSettings>(builder.Configuration.GetSection("PagamentoApiSettings"));
builder.Services.Configure<ProdutoApiSettings>(builder.Configuration.GetSection("ProdutoApiSettings"));
builder.Services.AddHttpServices();
builder.Services.AddAdapterPagamentoApi();
builder.Services.AddAdapterProdutoApi();
builder.Services.AddAdapterDatabaseRepositories();
builder.Services.AddDomainControllers();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.DescribeAllParametersInCamelCase();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SnackTech", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

if (string.IsNullOrEmpty(dbConnectionString))
{
    throw new InvalidOperationException(
        "Could not find a connection string named 'DefaultConnection'.");
}

builder.Services.AddDbContext<RepositoryDbContext>(options =>
    options.UseSqlServer(dbConnectionString));
builder.Services.AddHealthChecks()
    .ConfigureSQLHealthCheck();

var app = builder.Build();

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
await dbContext.Database.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SnackTech Order API v1");
});

//app.UseHttpsRedirection();
app.UseCustomHealthChecks();
app.UseAuthorization();

// Redirecionamento da URL raiz para /swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapControllers();

app.Run();
