namespace SnackTech.Orders.Common.Dto.ApiSources.Payments
{
    public class PagamentoApiSettings
    {
        public bool EnableIntegration { get; set; }
        public string UrlBase { get; set; } = null!;
    }
}