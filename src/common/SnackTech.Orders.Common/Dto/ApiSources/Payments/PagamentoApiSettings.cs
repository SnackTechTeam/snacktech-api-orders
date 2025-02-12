using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.ApiSources.Payments
{
    [ExcludeFromCodeCoverage]
    public class PagamentoApiSettings
    {
        public bool EnableIntegration { get; set; }
        public string UrlBase { get; set; } = null!;
        public string EndpointOperacao { get; set; } = null!;
    }
}