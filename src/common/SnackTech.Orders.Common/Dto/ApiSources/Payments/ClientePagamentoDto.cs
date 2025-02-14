using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.ApiSources.Payments
{
    [ExcludeFromCodeCoverage]
    public class ClientePagamentoDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}