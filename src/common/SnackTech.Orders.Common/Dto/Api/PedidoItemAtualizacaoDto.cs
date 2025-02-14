using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.Api;

[ExcludeFromCodeCoverage]
public class PedidoItemAtualizacaoDto
{
    //Informar o Id caso seja uma atualização do item ou null para item novo no pedido
    public string? IdentificacaoItem { get; set; }
    public int Quantidade { get; set; }
    public string Observacao { get; set; }
    public string IdentificacaoProduto { get; set; }
}
