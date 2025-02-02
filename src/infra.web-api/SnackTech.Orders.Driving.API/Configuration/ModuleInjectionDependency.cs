using SnackTech.Orders.Core.Controllers;
using SnackTech.Orders.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Driver.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ModuleInjectionDependency
    {
        public static IServiceCollection AddDomainControllers(this IServiceCollection services)
        {
            services.AddTransient<IClienteController, ClienteController>();
            services.AddTransient<IPedidoController, PedidoController>();

            return services;
        }        
    }
}