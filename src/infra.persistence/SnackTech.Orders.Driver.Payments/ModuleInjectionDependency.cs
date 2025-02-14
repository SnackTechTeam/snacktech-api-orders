using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Driver.Payments.HttpApiClients;
using SnackTech.Orders.Driver.Payments.Services;
using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Driver.Payments
{
    [ExcludeFromCodeCoverage]
    public static class ModuleInjectionDependency
    {
        public static IServiceCollection AddAdapterPagamentoApi(this IServiceCollection services)
        {
            services.AddRefitClient<IPagamentoHttpClient>()
                .ConfigureHttpClient((sp, client) =>
                {
                    // Obtém as configurações injetadas pelo Options
                    var settings = sp.GetRequiredService<IOptions<PagamentoApiSettings>>().Value;
                    client.BaseAddress = new Uri(settings.UrlBase);
                });

            services.AddTransient<IPagamentoApi, PagamentoApi>();

            return services;
        }
    }
}