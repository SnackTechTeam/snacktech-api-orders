using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.DataSource;

namespace SnackTech.Orders.Common.Interfaces.ApiSources
{
    public interface IPagamentoApi
    {
        Task<ResultadoOperacao<string>> IntegrarPedido(PedidoDto pedido);
    }
}
