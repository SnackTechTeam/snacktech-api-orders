using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Core.Domain.Types;

namespace SnackTech.Orders.Core.Interfaces
{
    internal interface IProdutoGateway
    {
        Task<ResultadoOperacao<ProdutoDto>> BuscarProdutoAsync(GuidValido id);
    }
}