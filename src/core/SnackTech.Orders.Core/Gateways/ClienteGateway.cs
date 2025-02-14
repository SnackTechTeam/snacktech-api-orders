using SnackTech.Orders.Common.Dto.DataSource;
using SnackTech.Orders.Common.Interfaces.DataSources;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;
using SnackTech.Orders.Core.Interfaces;

namespace SnackTech.Orders.Core.Gateways;

internal class ClienteGateway(IClienteDataSource dataSource) : IClienteGateway
{
    public async Task<bool> CadastrarNovoCliente(Cliente entidade)
    {
        ClienteDto dto = ConverterParaDto(entidade);

        return await dataSource.InserirClienteAsync(dto);
    }

    public async Task<Cliente?> ProcurarClientePorCpf(CpfValido cpf)
    {
        var clienteDto = await dataSource.PesquisarPorCpfAsync(cpf);

        if (clienteDto == null || clienteDto.Id == Guid.Empty)
        {
            return null;
        }

        return ConverterParaEntidade(clienteDto);
    }

    public async Task<Cliente?> ProcurarClientePorEmail(EmailValido emailCliente)
    {
        var clienteDto = await dataSource.PesquisarPorEmailAsync(emailCliente);

        if (clienteDto == null || clienteDto.Id == Guid.Empty)
        {
            return null;
        }

        return ConverterParaEntidade(clienteDto);
    }

    public async Task<Cliente?> ProcurarClientePorIdentificacao(GuidValido identificacao)
    {
        var clienteDto = await dataSource.PesquisarPorIdAsync(identificacao);

        if (clienteDto == null || clienteDto.Id == Guid.Empty)
        {
            return null;
        }

        return ConverterParaEntidade(clienteDto);
    }

    public static ClienteDto ConverterParaDto(Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome.Valor,
            Email = cliente.Email.Valor,
            Cpf = cliente.Cpf.Valor
        };
    }

    public static Cliente ConverterParaEntidade(ClienteDto clienteDto)
    {
        return new Cliente(clienteDto.Id, clienteDto.Nome, clienteDto.Email, clienteDto.Cpf);
    }
}
