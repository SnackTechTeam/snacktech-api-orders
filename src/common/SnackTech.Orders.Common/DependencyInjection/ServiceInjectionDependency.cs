using Microsoft.Extensions.DependencyInjection;
using SnackTech.Orders.Common.Http.Refit;
using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Driver.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ServiceInjectionDependency
    {
        public static IServiceCollection AddHttpServices(this IServiceCollection services)
        {
            services.AddScoped<IRequestExecutorHelper, BaseRequestExecutorHelper>();

            return services;
        }
    }
}