namespace SnackTech.Orders.Common.Dto.ApiSources.Payments
{
    public class ClientePagamentoDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}