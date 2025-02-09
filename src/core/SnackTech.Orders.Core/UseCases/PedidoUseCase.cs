using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;
using SnackTech.Orders.Core.Gateways;
using SnackTech.Orders.Core.Interfaces;
using SnackTech.Orders.Core.Presenters;

namespace SnackTech.Orders.Core.UseCases;

internal static class PedidoUseCase
{
    internal static async Task<ResultadoOperacao<Guid>> IniciarPedido(string? cpfCliente, IPedidoGateway pedidoGateway, IClienteGateway clienteGateway)
    {
        try
        {
            var cpf = cpfCliente is null or "" ? Cliente.CPF_CLIENTE_PADRAO : cpfCliente;
            var cliente = await clienteGateway.ProcurarClientePorCpf(cpf);
            if (cliente is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<Guid>($"Não foi possível iniciar um novo pedido para o cliente com CPF '{cpf}'.");
            }

            var entidade = new Pedido(Guid.NewGuid(), DateTime.Now, StatusPedidoValido.Iniciado, cliente);

            var foiCadastrado = await pedidoGateway.CadastrarNovoPedido(entidade);
            var retorno = foiCadastrado ?
                                PedidoPresenter.ApresentarResultadoPedidoIniciado(entidade) :
                                GeralPresenter.ApresentarResultadoErroLogico<Guid>($"Não foi possível iniciar o pedido para o cliente com CPF {cpfCliente}.");

            return retorno;
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<Guid>(ex);
        }
    }

    internal static async Task<ResultadoOperacao<PedidoRetornoDto>> BuscarPorIdenticacao(string identificacao, IPedidoGateway pedidoGateway)
    {
        try
        {
            var pedido = await pedidoGateway.PesquisarPorIdentificacao(identificacao);

            var retorno = pedido is null ?
                                GeralPresenter.ApresentarResultadoErroLogico<PedidoRetornoDto>($"Não foi possível localizar um pedido com identificação {identificacao}.") :
                                PedidoPresenter.ApresentarResultadoPedido(pedido);

            return retorno;
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<PedidoRetornoDto>(ex);
        }
    }

    internal static async Task<ResultadoOperacao<PedidoRetornoDto>> BuscarUltimoPedidoCliente(string? cpfCliente, IPedidoGateway pedidoGateway, IClienteGateway clienteGateway)
    {
        try
        {
            var cpf = cpfCliente is null or "" ? Cliente.CPF_CLIENTE_PADRAO : cpfCliente;
            var cliente = await clienteGateway.ProcurarClientePorCpf(cpf);
            if (cliente is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoRetornoDto>($"Não foi possível localizar o cliente com CPF '{cpf}'.");
            }

            var ultimosPedidos = await pedidoGateway.PesquisarPedidosPorCliente(cliente.Id);
            var ultimoPedido = ultimosPedidos.OrderBy(p => p.DataCriacao.Valor).LastOrDefault();

            var retorno = ultimoPedido is null ?
                                GeralPresenter.ApresentarResultadoErroLogico<PedidoRetornoDto>($"Não foi possível encontrar um pedido para o cliente com CPF {cpfCliente}.") :
                                PedidoPresenter.ApresentarResultadoPedido(ultimoPedido);

            return retorno;
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<PedidoRetornoDto>(ex);
        }
    }

    internal static async Task<ResultadoOperacao<IEnumerable<PedidoRetornoDto>>> ListarPedidosParaPagamento(IPedidoGateway pedidoGateway)
    {
        try
        {
            var pedidos = await pedidoGateway.PesquisarPedidosPorStatus(StatusPedidoValido.AguardandoPagamento);

            var retorno = PedidoPresenter.ApresentarResultadoPedido(pedidos);

            return retorno;
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<IEnumerable<PedidoRetornoDto>>(ex);
        }
    }

    internal static async Task<ResultadoOperacao<PagamentoDto>> FinalizarPedidoParaPagamento(string identificacao, IPedidoGateway pedidoGateway, IPagamentoGateway? pagamentoGateway)
    {
        try
        {
            var pedido = await pedidoGateway.PesquisarPorIdentificacao(identificacao);

            if (pedido is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PagamentoDto>($"Não foi possível finalizar para pagamento o pedido com identificação {identificacao}. Pedido não localizado.");
            }

            pedido.FecharPedidoParaPagamento();

            var foiAtualizado = await pedidoGateway.AtualizarStatusPedido(pedido);

            if (!foiAtualizado)
                return GeralPresenter.ApresentarResultadoErroLogico<PagamentoDto>($"Não foi possível finalizar para pagamento o pedido com identificação {identificacao}.");

            var qrCode = string.Empty;
            if (pagamentoGateway is not null)
            {
                var resultado = await pagamentoGateway.CriarPagamentoAsync(pedido);

                if (resultado.TeveExcecao())
                {
                    throw resultado.Excecao;
                }

                if (!resultado.TeveSucesso())
                {
                    return GeralPresenter.ApresentarResultadoErroLogico<PagamentoDto>(resultado.Mensagem);
                }

                var dadoPagamento = resultado.RecuperarDados();
                qrCode = dadoPagamento.QrCode;
            }

            return PedidoPresenter.ApresentarResultadoPedido(pedido, qrCode);
        }
        catch (ArgumentException ex)
        {
            return GeralPresenter.ApresentarResultadoErroLogico<PagamentoDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<PagamentoDto>(ex);
        }
    }

    internal static async Task<ResultadoOperacao<PedidoRetornoDto>> AtualizarItensPedido(PedidoAtualizacaoDto pedidoAtualizado, IPedidoGateway pedidoGateway, IProdutoGateway produtoGateway)
    {
        try
        {
            var pedido = await pedidoGateway.PesquisarPorIdentificacao(pedidoAtualizado.IdentificacaoPedido);

            if (pedido is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoRetornoDto>($"Não foi possível encontrar um pedido com identificação {pedidoAtualizado.IdentificacaoPedido}.");
            }

            //itens atualizados com GUID e que não existem no pedido persistido
            var itensNovosComIds = pedidoAtualizado.PedidoItens
                .Where(item => item.IdentificacaoItem != null)
                .Where(itemAtualizado => !pedido.Itens.Any(item => item.Id == itemAtualizado.IdentificacaoItem))
                .ToList();

            if (itensNovosComIds.Count > 0)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoRetornoDto>($"Não é possivel atualizar itens que não existem no pedido. Por favor, remove a identificação dos itens novos para que eles sejam cadastrados corretamente.");
            }

            //remover itens do pedido que estejam ausentes no pedido atualizado
            pedido.Itens.RemoveAll(itemPedido => !pedidoAtualizado.PedidoItens.Any(itemAtualizado => itemAtualizado.IdentificacaoItem == itemPedido.Id));

            //validar itens do pedido atualizado
            List<PedidoItem> itensValidados = await ValidarItensPedido(pedidoAtualizado.PedidoItens, produtoGateway);

            //atualizar os itens do pedido
            pedido.Itens = itensValidados;

            var foiAtualizado = await pedidoGateway.AtualizarItensDoPedido(pedido);

            var retorno = foiAtualizado ?
                PedidoPresenter.ApresentarResultadoPedido(pedido) :
                GeralPresenter.ApresentarResultadoErroLogico<PedidoRetornoDto>($"Não foi possível atualizar os itens do pedido com identificação {pedidoAtualizado.IdentificacaoPedido}.");

            return retorno;
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<PedidoRetornoDto>(ex);
        }
    }

    private static async Task<List<PedidoItem>> ValidarItensPedido(IEnumerable<PedidoItemAtualizacaoDto> itens, IProdutoGateway produtoGateway)
    {
        var itensValidados = new List<PedidoItem>();

        foreach (var item in itens)
        {
            var resultado = await produtoGateway.BuscarProdutoAsync(item.IdentificacaoProduto);

            if (!resultado.TeveSucesso())
            {
                throw new ArgumentException($"Não existe produto com identificação {item.IdentificacaoProduto}.");
            }

            var produto = ProdutoGateway.ConverterParaEntidade(resultado.RecuperarDados());
            Guid identificacaoItem = item.IdentificacaoItem is null || item.IdentificacaoItem == string.Empty ? Guid.NewGuid() : new GuidValido(item.IdentificacaoItem);
            itensValidados.Add(new PedidoItem(identificacaoItem, produto, item.Quantidade, item.Observacao));
        }

        return itensValidados;
    }

    internal static async Task<ResultadoOperacao<IEnumerable<PedidoRetornoDto>>> ListarPedidosAtivos(IPedidoGateway pedidoGateway)
    {
        try
        {
            var pedidos = await pedidoGateway.PesquisarPedidosPorStatus([
                    StatusPedidoValido.Pronto,
                    StatusPedidoValido.EmPreparacao,
                    StatusPedidoValido.Recebido
                ]);

            //A ordem dos status no Enum já representa a sequencia desejada para os status.
            var pedidosOrdenados = pedidos
                .OrderByDescending(p => (int)p.Status)
                .ThenByDescending(p => p.DataCriacao.Valor)
                .ToList();
            var retorno = PedidoPresenter.ApresentarResultadoPedido(pedidosOrdenados);

            return retorno;
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<IEnumerable<PedidoRetornoDto>>(ex);
        }
    }

    internal static async Task<ResultadoOperacao> IniciarPreparacaoPedido(string identificacao, IPedidoGateway pedidoGateway)
    {
        try
        {
            var pedido = await pedidoGateway.PesquisarPorIdentificacao(identificacao);

            if (pedido is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico($"Não foi possível iniciar o preparo do pedido com identificação {identificacao}. Pedido não localizado.");
            }

            pedido.IniciarPreparacao();

            var foiAtualizado = await pedidoGateway.AtualizarStatusPedido(pedido);

            if (!foiAtualizado)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PagamentoDto>($"Não foi possível iniciar o preparo do pedido com identificação {identificacao}.");
            }

            return PedidoPresenter.ApresentarResultadoOk();
        }
        catch (ArgumentException ex)
        {
            return GeralPresenter.ApresentarResultadoErroLogico(ex.Message);
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno(ex);
        }
    }

    internal static async Task<ResultadoOperacao> ConcluirPreparacaoPedido(string identificacao, IPedidoGateway pedidoGateway)
    {
        try
        {
            var pedido = await pedidoGateway.PesquisarPorIdentificacao(identificacao);

            if (pedido is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico($"Não foi possível concluir o preparo do pedido com identificação {identificacao}. Pedido não localizado.");
            }

            pedido.ConcluirPreparacao();

            var foiAtualizado = await pedidoGateway.AtualizarStatusPedido(pedido);

            if (!foiAtualizado)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PagamentoDto>($"Não foi possível concluir o preparo do pedido com identificação {identificacao}.");
            }

            return PedidoPresenter.ApresentarResultadoOk();
        }
        catch (ArgumentException ex)
        {
            return GeralPresenter.ApresentarResultadoErroLogico(ex.Message);
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno(ex);
        }
    }

    internal static async Task<ResultadoOperacao> FinalizarPedido(string identificacao, IPedidoGateway pedidoGateway)
    {
        try
        {
            var pedido = await pedidoGateway.PesquisarPorIdentificacao(identificacao);

            if (pedido is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico($"Não foi possível finalizar o pedido com identificação {identificacao}. Pedido não localizado.");
            }

            pedido.Finalizar();

            var foiAtualizado = await pedidoGateway.AtualizarStatusPedido(pedido);

            if (!foiAtualizado)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PagamentoDto>($"Não foi possível finalizar o pedido com identificação {identificacao}.");
            }

            return PedidoPresenter.ApresentarResultadoOk();
        }
        catch (ArgumentException ex)
        {
            return GeralPresenter.ApresentarResultadoErroLogico(ex.Message);
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno(ex);
        }
    }
}
