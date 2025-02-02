using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SnackTech.Orders.Driver.DataBase.Context;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text.Json;

namespace SnackTech.Orders.Driver.API.Configuration.HealthChecks
{
    [ExcludeFromCodeCoverage]
    public static class HealthChecksExtensions
    {
        public static IHealthChecksBuilder ConfigureSQLHealthCheck(this IHealthChecksBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddDbContextCheck<RepositoryDbContext>(
                    name: "SQL Server",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["SnackTech-Order-API", "Database"]
                );

            return builder;
        }

        public static void UseCustomHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("api/health/ready", new HealthCheckOptions
            {
                Predicate = (check) => check.Name == "SQL Server",
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonSerializer.Serialize(
                        new
                        {
                            status = report.Status.ToString(),
                            check = report.Entries.Select(entry => new
                            {
                                name = entry.Key,
                                status = entry.Value.Status.ToString(),
                                exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                                duration = entry.Value.Duration.ToString()
                            })
                        }
                    );

                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });

            app.MapHealthChecks("api/health/live", new HealthCheckOptions
            {
                Predicate = (_) => false
            });
        }
    }
}