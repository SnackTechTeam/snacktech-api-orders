using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;

namespace SnackTech.Orders.Core.Interfaces
{
    internal interface IPedidoGateway
    {
        Task<bool> AtualizarItensDoPedido(Pedido pedido);
        Task<bool> AtualizarStatusPedido(Pedido pedido);
        Task<bool> CadastrarNovoPedido(Pedido entidade);
        Task<IEnumerable<Pedido>> PesquisarPedidosPorCliente(GuidValido clienteId);
        Task<IEnumerable<Pedido>> PesquisarPedidosPorStatus(StatusPedidoValido status);
        Task<IEnumerable<Pedido>> PesquisarPedidosPorStatus(StatusPedidoValido[] status);
        Task<Pedido?> PesquisarPorIdentificacao(GuidValido identificacao);
    }
}