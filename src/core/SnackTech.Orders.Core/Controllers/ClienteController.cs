using SnackTech.Orders.Core.Gateways;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Interfaces.DataSources;
using SnackTech.Orders.Core.Interfaces;
using SnackTech.Orders.Core.UseCases;

namespace SnackTech.Orders.Core.Controllers;

public class ClienteController(IClienteDataSource clienteDataSource) : IClienteController
{
    public async Task<ResultadoOperacao<ClienteDto>> CadastrarNovoCliente(ClienteSemIdDto clienteSemIdDto)
    {
        var gateway = new ClienteGateway(clienteDataSource);

        var novoCliente = await ClienteUseCase.CriarNovoCliente(clienteSemIdDto, gateway);

        return novoCliente;
    }

    public async Task<ResultadoOperacao<ClienteDto>> IdentificarPorCpf(string cpf)
    {
        var gateway = new ClienteGateway(clienteDataSource);

        var cliente = await ClienteUseCase.PesquisarPorCpf(cpf, gateway);

        return cliente;
    }

    public async Task<ResultadoOperacao<ClienteDto>> SelecionarClientePadrao()
    {
        var gateway = new ClienteGateway(clienteDataSource);

        var cliente = await ClienteUseCase.SelecionarClientePadrao(gateway);

        return cliente;
    }
}
