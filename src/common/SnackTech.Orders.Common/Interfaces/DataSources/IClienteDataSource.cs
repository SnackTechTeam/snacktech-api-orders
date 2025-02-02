using SnackTech.Orders.Common.Dto.DataSource;

namespace SnackTech.Orders.Common.Interfaces.DataSources;

public interface IClienteDataSource
{
    Task<ClienteDto?> PesquisarPorIdAsync(Guid identificacao);
    Task<ClienteDto?> PesquisarPorEmailAsync(string email);
    Task<ClienteDto?> PesquisarPorCpfAsync(string cpf);
    Task<bool> InserirClienteAsync(ClienteDto clienteNovo);
}
