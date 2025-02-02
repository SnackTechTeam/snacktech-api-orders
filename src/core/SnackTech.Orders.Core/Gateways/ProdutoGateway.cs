using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;

namespace SnackTech.Orders.Core.Gateways
{
    internal class ProdutoGateway(IProdutoApi produtoApi)
    {
        internal async Task<ResultadoOperacao<ProdutoDto>> BuscarProdutoAsync(GuidValido id)
        {
            return await produtoApi.BuscarProdutoAsync(id.Valor);
        }

        internal static Produto ConverterParaEntidade(ProdutoDto dto)
        {
            return new(dto.IdentificacaoProduto, dto.Valor);
        }
    }
}
