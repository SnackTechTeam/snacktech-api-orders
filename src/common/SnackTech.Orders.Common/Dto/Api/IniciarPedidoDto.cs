using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.Api
{
    [ExcludeFromCodeCoverage]
    public class IniciarPedidoDto
    {
        public string Cpf { get; set; } = string.Empty;
    }
}