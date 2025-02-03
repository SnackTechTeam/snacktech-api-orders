using Refit;
using SnackTech.Orders.Common.Dto.DataSource;

namespace SnackTech.Orders.Driver.Payments.HttpApiClients
{
    public interface IPagamentoHttpClient
    {
        [Post("/Pagamento")]
        Task<ApiResponse<string>> IntegrarPedido([Body] PedidoDto pedido);
    }
}
