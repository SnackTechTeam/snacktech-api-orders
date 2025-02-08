using SnackTech.Orders.Common.Dto.DataSource;
using SnackTech.Orders.Common.Interfaces.DataSources;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;
using SnackTech.Orders.Core.Interfaces;

namespace SnackTech.Orders.Core.Gateways;

internal class PedidoGateway(IPedidoDataSource dataSource) : IPedidoGateway
{
    public async Task<bool> CadastrarNovoPedido(Pedido entidade)
    {
        var pedidoDto = ConverterParaDto(entidade);

        return await dataSource.InserirPedidoAsync(pedidoDto);
    }

    public async Task<Pedido?> PesquisarPorIdentificacao(GuidValido identificacao)
    {
        var pedidoDto = await dataSource.PesquisarPorIdentificacaoAsync(identificacao);

        if (pedidoDto is null || pedidoDto.Id == Guid.Empty)
            return null;

        return ConverterParaEntidade(pedidoDto);
    }

    public async Task<IEnumerable<Pedido>> PesquisarPedidosPorCliente(GuidValido clienteId)
    {
        var pedidosDto = await dataSource.PesquisarPedidosPorClienteIdAsync(clienteId);

        return pedidosDto.Select(ConverterParaEntidade);
    }

    public async Task<IEnumerable<Pedido>> PesquisarPedidosPorStatus(StatusPedidoValido status)
    {
        var pedidosDto = await dataSource.PesquisarPedidosPorStatusAsync([status.Valor]);

        return pedidosDto.Select(ConverterParaEntidade);
    }

    public async Task<IEnumerable<Pedido>> PesquisarPedidosPorStatus(StatusPedidoValido[] status)
    {
        var arrayStatus = status.Select(s => s.Valor).ToArray();
        var pedidosDto = await dataSource.PesquisarPedidosPorStatusAsync(arrayStatus);

        return pedidosDto.Select(ConverterParaEntidade);
    }

    public async Task<bool> AtualizarStatusPedido(Pedido pedido)
    {
        var pedidoDto = ConverterParaDto(pedido);

        return await dataSource.AtualizarStatusPedidoAsync(pedidoDto);
    }

    public async Task<bool> AtualizarItensDoPedido(Pedido pedido)
    {
        var pedidoDto = ConverterParaDto(pedido);

        return await dataSource.AlterarItensDoPedidoAsync(pedidoDto);
    }

    public static PedidoDto ConverterParaDto(Pedido pedido)
    {
        return new PedidoDto
        {
            Id = pedido.Id,
            DataCriacao = pedido.DataCriacao.Valor,
            Status = pedido.Status.Valor,
            Cliente = ClienteGateway.ConverterParaDto(pedido.Cliente),
            Itens = pedido.Itens.Select(ConverterItemParaDto)
        };
    }

    public static PedidoItemDto ConverterItemParaDto(PedidoItem pedidoItem)
    {
        return new PedidoItemDto
        {
            Id = pedidoItem.Id,
            Quantidade = pedidoItem.Quantidade.Valor,
            Observacao = pedidoItem.Observacao,
            ValorTotal = pedidoItem.Valor(),
            ValorUnitario = pedidoItem.Produto.Valor,
            ProdutoId = pedidoItem.Produto.Id
        };
    }

    public static Pedido ConverterParaEntidade(PedidoDto pedidoDto)
    {
        return new Pedido(
            pedidoDto.Id,
            pedidoDto.DataCriacao,
            pedidoDto.Status,
            ClienteGateway.ConverterParaEntidade(pedidoDto.Cliente),
            pedidoDto.Itens.Select(ConverterItemParaEntidade).ToList()
        );
    }

    public static PedidoItem ConverterItemParaEntidade(PedidoItemDto pedidoItemDto)
    {
        return new PedidoItem(
            pedidoItemDto.Id,
            new Produto(pedidoItemDto.ProdutoId, pedidoItemDto.ValorUnitario),
            pedidoItemDto.Quantidade,
            pedidoItemDto.Observacao
        );
    }
}
