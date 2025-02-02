using SnackTech.Orders.Common.Dto.Api;

namespace SnackTech.Orders.Common.Interfaces.ApiSources
{
    public interface IProdutoApi
    {
        Task<ResultadoOperacao<ProdutoDto>> BuscarProdutoAsync(Guid id);
    }
}