using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Http.Refit;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Driver.Payments.HttpApiClients;

namespace SnackTech.Orders.Driver.Payments.Services
{
    public class PagamentoApi(IPagamentoHttpClient _pagamentoHttpClient, IRequestExecutorHelper _requestExecutorHelper) : IPagamentoApi
    {
        public async Task<ResultadoOperacao<PagamentoDto>> CriarPagamentoAsync(PedidoPagamentoDto pedido)
        {
            return await _requestExecutorHelper.Execute(() => _pagamentoHttpClient.CriarPagamentoAsync(pedido));
        }
    }
}
