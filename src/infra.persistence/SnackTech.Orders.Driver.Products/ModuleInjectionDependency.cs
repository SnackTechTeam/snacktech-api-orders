using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using SnackTech.Orders.Common.Dto.ApiSources.Products;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Driver.Products.HttpApiClients;
using SnackTech.Orders.Driver.Products.Services;
using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Driver.Products
{
    [ExcludeFromCodeCoverage]
    public static class ModuleInjectionDependency
    {
        public static IServiceCollection AddAdapterProdutoApi(this IServiceCollection services)
        {
            services.AddRefitClient<IProdutoHttpClient>()
                .ConfigureHttpClient((sp, client) =>
                {
                    // Obtém as configurações injetadas pelo Options
                    var settings = sp.GetRequiredService<IOptions<ProdutoApiSettings>>().Value;
                    client.BaseAddress = new Uri(settings.UrlBase);
                });

            services.AddTransient<IProdutoApi, ProdutoApi>();

            return services;
        }
    }
}