using SnackTech.Orders.Common.Dto.Api;

namespace SnackTech.Orders.Core.Interfaces;

public interface IClienteController
{
    Task<ResultadoOperacao<ClienteDto>> CadastrarNovoCliente(ClienteSemIdDto clienteSemIdDto);
    Task<ResultadoOperacao<ClienteDto>> IdentificarPorCpf(string cpf);
    Task<ResultadoOperacao<ClienteDto>> SelecionarClientePadrao();
}