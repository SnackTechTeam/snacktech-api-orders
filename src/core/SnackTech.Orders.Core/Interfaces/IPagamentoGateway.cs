using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Core.Domain.Entities;

namespace SnackTech.Orders.Core.Interfaces
{
    internal interface IPagamentoGateway
    {
        Task<ResultadoOperacao<PagamentoDto>> CriarPagamentoAsync(Pedido pedido);
    }
}