using SnackTech.Orders.Core.Gateways;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Interfaces.DataSources;
using SnackTech.Orders.Core.Interfaces;
using SnackTech.Orders.Core.UseCases;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Core.Controllers;

[ExcludeFromCodeCoverage]
public class PedidoController(IPedidoDataSource pedidoDataSource,
                                IClienteDataSource clienteDataSource,
                                IPagamentoApi pagamentoApi,
                                IProdutoApi produtoApi,
                                IOptions<PagamentoApiSettings> pagamentoApiSettings) : IPedidoController
{
    public async Task<ResultadoOperacao<Guid>> IniciarPedido(string? cpfCliente)
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);
        var clienteGateway = new ClienteGateway(clienteDataSource);

        var identificacaoPedido = await PedidoUseCase.IniciarPedido(cpfCliente, pedidoGateway, clienteGateway);

        return identificacaoPedido;
    }

    public async Task<ResultadoOperacao<PedidoRetornoDto>> BuscarPorIdenticacao(string identificacao)
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);
        var pedido = await PedidoUseCase.BuscarPorIdenticacao(identificacao, pedidoGateway);

        return pedido;
    }

    public async Task<ResultadoOperacao<PedidoRetornoDto>> BuscarUltimoPedidoCliente(string cpfCliente)
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);
        var clienteGateway = new ClienteGateway(clienteDataSource);

        var pedido = await PedidoUseCase.BuscarUltimoPedidoCliente(cpfCliente, pedidoGateway, clienteGateway);

        return pedido;
    }

    public async Task<ResultadoOperacao<IEnumerable<PedidoRetornoDto>>> ListarPedidosParaPagamento()
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);

        var pedidos = await PedidoUseCase.ListarPedidosParaPagamento(pedidoGateway);

        return pedidos;
    }

    public async Task<ResultadoOperacao<PagamentoDto>> FinalizarPedidoParaPagamento(string identificacao)
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);
        PagamentoGateway? pagamentoGateway = null;

        if (pagamentoApiSettings.Value.EnableIntegration)
        {
            pagamentoGateway = new PagamentoGateway(pagamentoApi);
        }

        var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(identificacao, pedidoGateway, pagamentoGateway);

        return resultado;
    }

    public async Task<ResultadoOperacao<PedidoRetornoDto>> AtualizarPedido(PedidoAtualizacaoDto pedidoAtualizado)
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);
        var produtoGateway = new ProdutoGateway(produtoApi);

        var resultado = await PedidoUseCase.AtualizarItensPedido(pedidoAtualizado, pedidoGateway, produtoGateway);

        return resultado;
    }

    public async Task<ResultadoOperacao<IEnumerable<PedidoRetornoDto>>> ListarPedidosAtivos()
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);

        var pedidos = await PedidoUseCase.ListarPedidosAtivos(pedidoGateway);

        return pedidos;
    }

    public async Task<ResultadoOperacao> IniciarPreparacaoPedido(string identificacao)
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);

        var resultado = await PedidoUseCase.IniciarPreparacaoPedido(identificacao, pedidoGateway);

        return resultado;
    }

    public async Task<ResultadoOperacao> ConcluirPreparacaoPedido(string identificacao)
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);

        var resultado = await PedidoUseCase.ConcluirPreparacaoPedido(identificacao, pedidoGateway);

        return resultado;
    }

    public async Task<ResultadoOperacao> FinalizarPedido(string identificacao)
    {
        var pedidoGateway = new PedidoGateway(pedidoDataSource);

        var resultado = await PedidoUseCase.FinalizarPedido(identificacao, pedidoGateway);

        return resultado;
    }
}
