using Refit;
using SnackTech.Orders.Common.Dto.Api;

namespace SnackTech.Orders.Driver.Products.HttpApiClients
{
    public interface IProdutoHttpClient
    {
        [Get("/Produtos/{id}")]
        Task<ApiResponse<ProdutoDto>> BuscarProdutoAsync(Guid id);
    }
}