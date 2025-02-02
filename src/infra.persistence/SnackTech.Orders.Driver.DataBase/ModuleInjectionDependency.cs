using Microsoft.Extensions.DependencyInjection;
using SnackTech.Orders.Common.Interfaces.DataSources;
using System.Diagnostics.CodeAnalysis;
using SnackTech.Orders.Driver.DataBase.DataSources;

namespace SnackTech.Orders.Driver.DataBase
{
    [ExcludeFromCodeCoverage]
    public static class ModuleInjectionDependency
    {
        public static IServiceCollection AddAdapterDatabaseRepositories(this IServiceCollection services)
        {

            services.AddTransient<IClienteDataSource, ClienteDataSource>();
            services.AddTransient<IPedidoDataSource, PedidoDataSource>();

            return services;
        }
    }
}