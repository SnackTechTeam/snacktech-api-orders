using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Http.Refit;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Driver.Payments.HttpApiClients;

namespace SnackTech.Orders.Driver.Payments.Services
{
    public class PagamentoApi(IPagamentoHttpClient _pagamentoHttpClient, IRequestExecutorHelper _requestExecutorHelper) : IPagamentoApi
    {
        public Task<ResultadoOperacao<PagamentoDto>> CriarPagamento(PedidoPagamentoDto pedido)
        {
            return _requestExecutorHelper.Execute(() => _pagamentoHttpClient.CriarPagamento(pedido));
        }
    }
}
