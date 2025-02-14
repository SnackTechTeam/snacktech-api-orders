using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.Api
{
    [ExcludeFromCodeCoverage]
    public class PagamentoDto
    {
        public Guid Id { get; set; }
        public string QrCode { get; set; } = default!;
        public decimal ValorTotal { get; set; }
    }
}