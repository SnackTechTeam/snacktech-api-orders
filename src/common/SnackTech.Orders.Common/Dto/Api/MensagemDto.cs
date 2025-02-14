using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.Api
{
    [ExcludeFromCodeCoverage]
    public class MensagemDto(string mensagem)
    {
        public string Mensagem { get; set; } = mensagem;
    }
}