using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;

namespace SnackTech.Orders.Core.Interfaces
{
    internal interface IClienteGateway
    {
        Task<bool> CadastrarNovoCliente(Cliente entidade);
        Task<Cliente?> ProcurarClientePorCpf(CpfValido cpf);
        Task<Cliente?> ProcurarClientePorEmail(EmailValido emailCliente);
        Task<Cliente?> ProcurarClientePorIdentificacao(GuidValido identificacao);
    }
}