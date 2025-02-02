using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;
using SnackTech.Orders.Core.Gateways;
using SnackTech.Orders.Core.Presenters;

namespace SnackTech.Orders.Core.UseCases;

internal static class PedidoUseCase
{
    internal static async Task<ResultadoOperacao<Guid>> IniciarPedido(string? cpfCliente, PedidoGateway pedidoGateway, ClienteGateway clienteGateway)
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

    internal static async Task<ResultadoOperacao<PedidoRetornoDto>> BuscarPorIdenticacao(string identificacao, PedidoGateway pedidoGateway)
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

    internal static async Task<ResultadoOperacao<PedidoRetornoDto>> BuscarUltimoPedidoCliente(string cpfCliente, PedidoGateway pedidoGateway, ClienteGateway clienteGateway)
    {
        try
        {
            var cpf = cpfCliente is null or "" ? Cliente.CPF_CLIENTE_PADRAO : cpfCliente;
            var cliente = await clienteGateway.ProcurarClientePorCpf(cpf);
            if (cliente is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoRetornoDto>($"Não foi possível iniciar um novo pedido para o cliente com CPF '{cpf}'.");
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

    internal static async Task<ResultadoOperacao<IEnumerable<PedidoRetornoDto>>> ListarPedidosParaPagamento(PedidoGateway pedidoGateway)
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

    internal static async Task<ResultadoOperacao<PedidoPagamentoDto>> FinalizarPedidoParaPagamento(string identificacao, PedidoGateway pedidoGateway, PagamentoGateway pagamentoGateway)
    {
        try
        {
            var pedido = await pedidoGateway.PesquisarPorIdentificacao(identificacao);

            if (pedido is null)
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoPagamentoDto>($"Não foi possível finalizar para pagamento o pedido com identificação {identificacao}. Pedido não localizado.");
            }

            pedido.FecharPedidoParaPagamento();

            var foiAtualizado = await pedidoGateway.AtualizarStatusPedido(pedido);

            if (!foiAtualizado)
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoPagamentoDto>($"Não foi possível finalizar para pagamento o pedido com identificação {identificacao}.");

            //TODO: Fazer aqui o envio para a API de Pagamentos?
            var resultado = await pagamentoGateway.IntegrarPedidoAsync(pedido);

            if (resultado.TeveExcecao())
            {
                throw resultado.Excecao;
            }

            if (!resultado.TeveSucesso())
            {
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoPagamentoDto>(resultado.Mensagem);
            }

            var dadoPagamento = resultado.RecuperarDados();

            return PedidoPresenter.ApresentarResultadoPedido(pedido, dadoPagamento);
        }
        catch (ArgumentException ex)
        {
            return GeralPresenter.ApresentarResultadoErroLogico<PedidoPagamentoDto>(ex.Message);
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<PedidoPagamentoDto>(ex);
        }
    }

    internal static async Task<ResultadoOperacao<PedidoRetornoDto>> AtualizarItensPedido(PedidoAtualizacaoDto pedidoAtualizado, PedidoGateway pedidoGateway, ProdutoGateway produtoGateway)
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
                GeralPresenter.ApresentarResultadoErroLogico<PedidoRetornoDto>($"Não é possivel atualizar itens que não existem no pedido. Por favor, remove a identificação dos itens novos para que eles sejam cadastrados corretamente.");
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

    private static async Task<List<PedidoItem>> ValidarItensPedido(IEnumerable<PedidoItemAtualizacaoDto> itens, ProdutoGateway produtoGateway)
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

    internal static async Task<ResultadoOperacao<IEnumerable<PedidoRetornoDto>>> ListarPedidosAtivos(PedidoGateway pedidoGateway)
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
            var retorno = PedidoPresenter.ApresentarResultadoPedido(pedidos);

            return retorno;
        }
        catch (Exception ex)
        {
            return GeralPresenter.ApresentarResultadoErroInterno<IEnumerable<PedidoRetornoDto>>(ex);
        }
    }

    internal static async Task<ResultadoOperacao> IniciarPreparacaoPedido(string identificacao, PedidoGateway pedidoGateway)
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
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoPagamentoDto>($"Não foi possível iniciar o preparo o pedido com identificação {identificacao}.");
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

    internal static async Task<ResultadoOperacao> ConcluirPreparacaoPedido(string identificacao, PedidoGateway pedidoGateway)
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
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoPagamentoDto>($"Não foi possível concluir o preparo o pedido com identificação {identificacao}.");
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

    internal static async Task<ResultadoOperacao> FinalizarPedido(string identificacao, PedidoGateway pedidoGateway)
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
                return GeralPresenter.ApresentarResultadoErroLogico<PedidoPagamentoDto>($"Não foi possível finalizar o pedido com identificação {identificacao}.");
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
