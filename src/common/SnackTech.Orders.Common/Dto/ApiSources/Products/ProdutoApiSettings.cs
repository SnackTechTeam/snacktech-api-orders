using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.ApiSources.Products
{
    [ExcludeFromCodeCoverage]
    public class ProdutoApiSettings
    {
        public bool EnableIntegration { get; set; }
        public string UrlBase { get; set; } = null!;
    }
}
