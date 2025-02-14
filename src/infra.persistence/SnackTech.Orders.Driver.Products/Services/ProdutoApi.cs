using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Http.Refit;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Driver.Products.HttpApiClients;

namespace SnackTech.Orders.Driver.Products.Services
{
    public class ProdutoApi(IProdutoHttpClient _produtoHttpClient, IRequestExecutorHelper _requestExecutorHelper) : IProdutoApi
    {
        public async Task<ResultadoOperacao<ProdutoDto>> BuscarProdutoAsync(Guid id)
        {
            return await _requestExecutorHelper.Execute(() => _produtoHttpClient.BuscarProdutoAsync(id));
        }
    }
}
