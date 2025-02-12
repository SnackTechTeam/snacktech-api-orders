using FluentAssertions;
using Moq;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.DataSource;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;
using SnackTech.Orders.Core.Interfaces;
using SnackTech.Orders.Core.UseCases;

namespace SnackTech.Orders.Core.Tests.UseCases
{
    public class PedidoUseCaseTest
    {
        private readonly Mock<IPedidoGateway> _pedidoGatewayMock;
        private readonly Mock<IClienteGateway> _clienteGatewayMock;
        private readonly Mock<IPagamentoGateway> _pagamentoGatewayMock;
        private readonly Mock<IProdutoGateway> _produtoGatewayMock;

        public PedidoUseCaseTest()
        {
            _pedidoGatewayMock = new Mock<IPedidoGateway>();
            _clienteGatewayMock = new Mock<IClienteGateway>();
            _pagamentoGatewayMock = new Mock<IPagamentoGateway>();
            _produtoGatewayMock = new Mock<IProdutoGateway>();
        }

        #region IniciarPedido
        [Fact]
        public async Task IniciarPedido_DeveRetornarSucesso_QuandoClienteExisteEPedidoCadastrado()
        {
            // Arrange
            var cpf = "00000000191";
            var cliente = new Cliente(Guid.NewGuid(), "nome", "email@email.com", cpf);

            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpf))
                .ReturnsAsync(cliente);

            _pedidoGatewayMock.Setup(p => p.CadastrarNovoPedido(It.IsAny<Pedido>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await PedidoUseCase.IniciarPedido(cpf, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(true);
            resultado.Dados.Should().NotBeEmpty();
        }

        [Fact]
        public async Task IniciarPedido_DeveRetornarSucesso_QuandoCpfNullEPedidoCadastrado()
        {
            // Arrange
            var cpf = "00000000191";
            var clientePadrao = new Cliente(Guid.NewGuid(), "nome", "email@email.com", cpf);

            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpf))
                .ReturnsAsync(clientePadrao);

            _pedidoGatewayMock.Setup(p => p.CadastrarNovoPedido(It.IsAny<Pedido>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await PedidoUseCase.IniciarPedido(null, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(true);
            resultado.Dados.Should().NotBeEmpty();
        }

        [Fact]
        public async Task IniciarPedido_DeveRetornarErro_QuandoClienteNaoExiste()
        {
            // Arrange
            var cpfCliente = "00000000191";
            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpfCliente))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await PedidoUseCase.IniciarPedido(cpfCliente, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(false);
            resultado.Mensagem.Should().Contain("Não foi possível iniciar um novo pedido");
        }

        [Fact]
        public async Task IniciarPedido_DeveRetornarErro_QuandoPedidoNaoForCadastrado()
        {
            // Arrange
            var cpf = "00000000191";
            var cliente = new Cliente(Guid.NewGuid(), "nome", "email@email.com", cpf);

            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpf))
                .ReturnsAsync(cliente);

            _pedidoGatewayMock.Setup(p => p.CadastrarNovoPedido(It.IsAny<Pedido>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await PedidoUseCase.IniciarPedido(cpf, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(false);
            resultado.Mensagem.Should().Contain("Não foi possível iniciar o pedido");
        }

        [Fact]
        public async Task IniciarPedido_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            var cpfCliente = "00000000191";
            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpfCliente))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.IniciarPedido(cpfCliente, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(false);
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }
        #endregion

        #region BuscarPorIdenticacao
        [Fact]
        public async Task BuscarPorIdenticacao_DeveRetornarPedido_QuandoIdentificacaoExistir()
        {
            //Arrange
            var pedido = CriarPedidoValido();
            var id = pedido.Id;

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            //Act
            var resultado = await PedidoUseCase.BuscarPorIdenticacao(id, _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(true);
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.IdentificacaoPedido.Should().Be(id.Valor);
        }

        [Fact]
        public async Task BuscarPorIdenticacao_DeveRetornarErro_QuandoIdentificacaoNaoExistir()
        {
            //Arrange
            var id = Guid.NewGuid();
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync((Pedido?)null);

            //Act
            var resultado = await PedidoUseCase.BuscarPorIdenticacao(id.ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(false);
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Não foi possível localizar um pedido com identificação");
        }

        [Fact]
        public async Task BuscarPorIdenticacao_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            //Arrange
            var id = Guid.NewGuid();
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            //Act
            var resultado = await PedidoUseCase.BuscarPorIdenticacao(id.ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(false);
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }
        #endregion

        #region BuscarUltimoPedidoCliente
        [Fact]
        public async Task BuscarUltimoPedidoCliente_DeveRetornarPedido_QuandoClienteEPedidoExistem()
        {
            // Arrange
            var cpfCliente = "00000000191";
            var cliente = CriarClienteValido();
            var pedidoMaisAntigo = new Pedido(Guid.NewGuid(), DateTime.Now.AddDays(-5), StatusPedidoValido.Iniciado, cliente);
            var pedidoMaisRecente = new Pedido(Guid.NewGuid(), DateTime.Now, StatusPedidoValido.AguardandoPagamento, cliente);

            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpfCliente))
                .ReturnsAsync(cliente);

            _pedidoGatewayMock.Setup(p => p.PesquisarPedidosPorCliente(cliente.Id))
                .ReturnsAsync([pedidoMaisAntigo, pedidoMaisRecente]);

            // Act
            var resultado = await PedidoUseCase.BuscarUltimoPedidoCliente(cpfCliente, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);
            
            // Assert
            resultado.Sucesso.Should().Be(true);
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.IdentificacaoPedido.Should().Be(pedidoMaisRecente.Id.Valor);
        }

        [Fact]
        public async Task BuscarUltimoPedidoCliente_DeveRetornarPedido_QuandoCpfNull()
        {
            // Arrange
            var cpfCliente = Cliente.CPF_CLIENTE_PADRAO;
            var clientePadrao = CriarClienteValido();
            var pedidoMaisAntigo = new Pedido(Guid.NewGuid(), DateTime.Now.AddDays(-5), StatusPedidoValido.Iniciado, clientePadrao);
            var pedidoMaisRecente = new Pedido(Guid.NewGuid(), DateTime.Now, StatusPedidoValido.AguardandoPagamento, clientePadrao);

            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpfCliente))
                .ReturnsAsync(clientePadrao);

            _pedidoGatewayMock.Setup(p => p.PesquisarPedidosPorCliente(clientePadrao.Id))
                .ReturnsAsync([pedidoMaisAntigo, pedidoMaisRecente]);

            // Act
            var resultado = await PedidoUseCase.BuscarUltimoPedidoCliente(null, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(true);
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.IdentificacaoPedido.Should().Be(pedidoMaisRecente.Id.Valor);
        }

        [Fact]
        public async Task BuscarUltimoPedidoCliente_DeveRetornarErro_QuandoClienteNaoExiste()
        {
            // Arrange
            var cpfCliente = "00000000191";
            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpfCliente))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await PedidoUseCase.BuscarUltimoPedidoCliente(cpfCliente, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(false);
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Não foi possível localizar o cliente");            
        }

        [Fact]
        public async Task BuscarUltimoPedidoCliente_DeveRetornarErro_QuandoNaoExistemPedidosParaCliente()
        {
            // Arrange
            var cpfCliente = "00000000191";
            var cliente = CriarClienteValido();

            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpfCliente))
                .ReturnsAsync(cliente);

            _pedidoGatewayMock.Setup(p => p.PesquisarPedidosPorCliente(cliente.Id))
                .ReturnsAsync([]); // Lista vazia

            // Act
            var resultado = await PedidoUseCase.BuscarUltimoPedidoCliente(cpfCliente, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(false);
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Não foi possível encontrar um pedido para o cliente");            
        }

        [Fact]
        public async Task BuscarUltimoPedidoCliente_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            var cpfCliente = "00000000191";
            _clienteGatewayMock.Setup(c => c.ProcurarClientePorCpf(cpfCliente))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.BuscarUltimoPedidoCliente(cpfCliente, _pedidoGatewayMock.Object, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().Be(false);
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region ListarPedidosParaPagamento
        [Fact]
        public async Task ListarPedidosParaPagamento_DeveRetornarPedidos_QuandoExistiremPedidos()
        {
            // Arrange
            var pedidos = new List<Pedido>
            {
                CriarPedido(
                    CriarClienteValido(),
                    [CriarPedidoItem(
                        CriarProdutoValido())],
                        StatusPedidoValido.AguardandoPagamento
                    ),
                CriarPedido(
                    CriarClienteValido(),
                    [CriarPedidoItem(
                        CriarProdutoValido())],
                        StatusPedidoValido.AguardandoPagamento
                    )
            };

            _pedidoGatewayMock.Setup(p => p.PesquisarPedidosPorStatus(StatusPedidoValido.AguardandoPagamento))
                .ReturnsAsync(pedidos);

            // Act
            var resultado = await PedidoUseCase.ListarPedidosParaPagamento(_pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Should().HaveCount(2);
        }

        [Fact]
        public async Task ListarPedidosParaPagamento_DeveRetornarListaVazia_QuandoNaoExistiremPedidos()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPedidosPorStatus(StatusPedidoValido.AguardandoPagamento))
                .ReturnsAsync(new List<Pedido>());

            // Act
            var resultado = await PedidoUseCase.ListarPedidosParaPagamento(_pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task ListarPedidosParaPagamento_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPedidosPorStatus(StatusPedidoValido.AguardandoPagamento))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.ListarPedidosParaPagamento(_pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }
        #endregion

        #region FinalizarPedidoParaPagamento

        [Fact]
        public async Task FinalizarPedidoParaPagamento_DeveRetornarErro_QuandoPedidoNaoForEncontrado()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync((Pedido?)null);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object, _pagamentoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Pedido não localizado");
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamento_DeveFinalizarPedido_QuandoPagamentoNaoForNecessario()
        {
            // Arrange
            var pedido = CriarPedidoValido();            

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(true);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(pedido.Id.Valor.ToString(), _pedidoGatewayMock.Object, null);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.QrCode.Should().BeNullOrEmpty();
            pedido.Status.Valor.Should().Be(StatusPedidoValido.AguardandoPagamento);
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamento_DeveRetornarErro_QuandoAtualizacaoDeStatusFalhar()
        {
            // Arrange
            var pedido = CriarPedidoValido();

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(false);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object, _pagamentoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Não foi possível finalizar para pagamento");
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamento_DeveRetornarQrCode_QuandoPagamentoForBemSucedido()
        {
            // Arrange
            var pedido = CriarPedidoValido();
            var pagamentoDto = new PagamentoDto { QrCode = "QRCode123" };
            var resultadoOperacao = new ResultadoOperacao<PagamentoDto>(pagamentoDto);

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);
            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(true);
            _pagamentoGatewayMock.Setup(p => p.CriarPagamentoAsync(pedido))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(pedido.Id, _pedidoGatewayMock.Object, _pagamentoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.QrCode.Should().Be("QRCode123");
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamento_DeveRetornarErroInterno_QuandoCriacaoDoPagamentoFalhar()
        {
            // Arrange
            var pedido = CriarPedidoValido();
            var resultadoOperacao = new ResultadoOperacao<PagamentoDto>(new Exception("Erro inesperado"));            

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);
            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(true);
            _pagamentoGatewayMock.Setup(p => p.CriarPagamentoAsync(pedido))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object, _pagamentoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamento_DeveRetornarErro_QuandoCriacaoDoPagamentoFalhar()
        {
            // Arrange
            var pedido = CriarPedidoValido();
            var resultadoOperacao = new ResultadoOperacao<PagamentoDto>("Erro Logico", true);

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);
            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(true);
            _pagamentoGatewayMock.Setup(p => p.CriarPagamentoAsync(pedido))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(pedido.Id, _pedidoGatewayMock.Object, _pagamentoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Erro Logico");
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamento_DeveRetornarErroInterno_QuandoOcorrerExcecaoDeArgumento()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new ArgumentException("Argumento inválido"));

            // Act
            var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object, _pagamentoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Argumento inválido");
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamento_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.FinalizarPedidoParaPagamento(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object, _pagamentoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region AtualizarItensPedido

        [Fact]
        public async Task AtualizarItensPedido_DeveRetornarErro_QuandoPedidoNaoForEncontrado()
        {
            // Arrange
            var pedidoAtualizado = new PedidoAtualizacaoDto() { IdentificacaoPedido = Guid.NewGuid().ToString() };

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync((Pedido?)null);

            // Act
            var resultado = await PedidoUseCase.AtualizarItensPedido(pedidoAtualizado, _pedidoGatewayMock.Object, _produtoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Não foi possível encontrar um pedido");
        }

        [Fact]
        public async Task AtualizarItensPedido_DeveRetornarErro_QuandoExistiremItensNovosComIdentificacao()
        {
            // Arrange
            var pedido = CriarPedidoValido();
            var pedidoAtualizado = new PedidoAtualizacaoDto
            {
                IdentificacaoPedido = pedido.Id.Valor.ToString(),
                PedidoItens = [
                    new() { IdentificacaoItem = Guid.NewGuid().ToString() } // Item com GUID que não existe no pedido                    
                    ]
            };

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            // Act
            var resultado = await PedidoUseCase.AtualizarItensPedido(pedidoAtualizado, _pedidoGatewayMock.Object, _produtoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Não é possivel atualizar itens que não existem no pedido");
        }

        [Fact]
        public async Task AtualizarItensPedido_DeveRemoverItensAusentesEAtualizarPedido()
        {
            // Arrange
            var pedido = CriarPedidoValido();
            var produto = new ResultadoOperacao<ProdutoDto>(new ProdutoDto());

            var pedidoAtualizado = new PedidoAtualizacaoDto
            {
                IdentificacaoPedido = pedido.Id.Valor.ToString(),
                PedidoItens = [] // Nenhum item no pedido atualizado
            };

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);
            
            _produtoGatewayMock.Setup(p => p.BuscarProdutoAsync(It.IsAny<GuidValido>()))
                .ReturnsAsync(produto);

            _pedidoGatewayMock.Setup(p => p.AtualizarItensDoPedido(pedido))
                .ReturnsAsync(true);

            // Act
            var resultado = await PedidoUseCase.AtualizarItensPedido(pedidoAtualizado, _pedidoGatewayMock.Object, _produtoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            pedido.Itens.Should().BeEmpty();
        }

        [Fact]
        public async Task AtualizarItensPedido_DeveIncluirItensAusentesEAtualizarPedido()
        {
            // Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [],
                StatusPedidoValido.Iniciado
            );

            var idProduto = Guid.NewGuid();
            var produtoDto = new ResultadoOperacao<ProdutoDto>(new ProdutoDto
            {
                IdentificacaoProduto = idProduto,
                Valor = 1
            });

            var pedidoAtualizado = new PedidoAtualizacaoDto
            {
                IdentificacaoPedido = pedido.Id.Valor.ToString(),
                PedidoItens =
                [
                    new PedidoItemAtualizacaoDto
                    {
                        IdentificacaoProduto = idProduto.ToString(),
                        Quantidade = 1
                    }
                ]
            };

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _produtoGatewayMock.Setup(p => p.BuscarProdutoAsync(It.IsAny<GuidValido>()))
                .ReturnsAsync(produtoDto);

            _pedidoGatewayMock.Setup(p => p.AtualizarItensDoPedido(pedido))
                .ReturnsAsync(true);

            // Act
            var resultado = await PedidoUseCase.AtualizarItensPedido(pedidoAtualizado, _pedidoGatewayMock.Object, _produtoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Itens.Count().Should().Be(1);
        }

        [Fact]
        public async Task AtualizarItensPedido_DeveRetornarErro_QuandoNaoForPossivelAtualizarItens()
        {
            // Arrange
            var pedido = CriarPedidoValido();

            var pedidoAtualizado = new PedidoAtualizacaoDto
            {
                IdentificacaoPedido = pedido.Id.Valor.ToString(),
                PedidoItens = [] // Nenhum item no pedido atualizado
            };

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarItensDoPedido(pedido))
                .ReturnsAsync(false);

            // Act
            var resultado = await PedidoUseCase.AtualizarItensPedido(pedidoAtualizado, _pedidoGatewayMock.Object, _produtoGatewayMock.Object);            

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Não foi possível atualizar os itens do pedido");
        }

        [Fact]
        public async Task AtualizarItensPedido_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            var pedidoAtualizado = new PedidoAtualizacaoDto { IdentificacaoPedido = Guid.NewGuid().ToString() };

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.AtualizarItensPedido(pedidoAtualizado, _pedidoGatewayMock.Object, _produtoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region ListarPedidosAtivos

        [Fact]
        public async Task ListarPedidosAtivos_DeveRetornarPedidosOrdenadosCorretamente()
        {
            // Arrange
            var cliente = CriarClienteValido();
            var pedidos = new List<Pedido>
            {
                new(Guid.NewGuid(), DateTime.Now.AddMinutes(-20), StatusPedidoValido.EmPreparacao, cliente),
                new(Guid.NewGuid(), DateTime.Now.AddMinutes(-30), StatusPedidoValido.Recebido, cliente),
                new(Guid.NewGuid(), DateTime.Now.AddMinutes(-10), StatusPedidoValido.Pronto, cliente)
            };

            _pedidoGatewayMock.Setup(p => p.PesquisarPedidosPorStatus(It.IsAny<StatusPedidoValido[]>()))
                .ReturnsAsync(pedidos);

            // Act
            var resultado = await PedidoUseCase.ListarPedidosAtivos(_pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().HaveCount(3);
            resultado.Dados.Select(p => p.Status).Should().ContainInOrder(
                StatusPedidoValido.Pronto, StatusPedidoValido.EmPreparacao, StatusPedidoValido.Recebido
            );
        }

        [Fact]
        public async Task ListarPedidosAtivos_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPedidosPorStatus(It.IsAny<StatusPedidoValido[]>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.ListarPedidosAtivos(_pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region IniciarPreparacaoPedido

        [Fact]
        public async Task IniciarPreparacaoPedido_DeveRetornarErro_QuandoPedidoNaoForEncontrado()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync((Pedido?)null);

            // Act
            var resultado = await PedidoUseCase.IniciarPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Pedido não localizado");
        }

        [Fact]
        public async Task IniciarPreparacaoPedido_DeveIniciarPreparacaoPedido_QuandoPedidoEncontrado()
        {
            // Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [
                    CriarPedidoItem(
                        CriarProdutoValido())
                ],
                StatusPedidoValido.Recebido);

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(true);

            // Act
            var resultado = await PedidoUseCase.IniciarPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            pedido.Status.Valor.Should().Be(StatusPedidoValido.EmPreparacao);
        }

        [Fact]
        public async Task IniciarPreparacaoPedido_DeveRetornarErro_QuandoAtualizacaoDeStatusFalhar()
        {
            // Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [
                    CriarPedidoItem(
                        CriarProdutoValido())
                ],
                StatusPedidoValido.Recebido);

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(false);

            // Act
            var resultado = await PedidoUseCase.IniciarPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Não foi possível iniciar o preparo do pedido");
        }

        [Fact]
        public async Task IniciarPreparacaoPedido_DeveRetornarErroInterno_QuandoOcorrerExcecaoDeArgumento()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new ArgumentException("Argumento inválido"));

            // Act
            var resultado = await PedidoUseCase.IniciarPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Argumento inválido");
        }

        [Fact]
        public async Task IniciarPreparacaoPedido_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.IniciarPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region ConcluirPreparacaoPedido

        [Fact]
        public async Task ConcluirPreparacaoPedido_DeveRetornarErro_QuandoPedidoNaoForEncontrado()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync((Pedido?)null);

            // Act
            var resultado = await PedidoUseCase.ConcluirPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Pedido não localizado");
        }

        [Fact]
        public async Task ConcluirPreparacaoPedido_DeveConcluirPreparacaoPedido_QuandoPedidoEncontrado()
        {
            // Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [
                    CriarPedidoItem(
                        CriarProdutoValido())
                ],
                StatusPedidoValido.EmPreparacao);

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(true);

            // Act
            var resultado = await PedidoUseCase.ConcluirPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            pedido.Status.Valor.Should().Be(StatusPedidoValido.Pronto);
        }

        [Fact]
        public async Task ConcluirPreparacaoPedido_DeveRetornarErro_QuandoAtualizacaoDeStatusFalhar()
        {
            // Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [
                    CriarPedidoItem(
                        CriarProdutoValido())
                ],
                StatusPedidoValido.EmPreparacao);

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(false);

            // Act
            var resultado = await PedidoUseCase.ConcluirPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Não foi possível concluir o preparo do pedido");
        }

        [Fact]
        public async Task ConcluirPreparacaoPedido_DeveRetornarErroInterno_QuandoOcorrerExcecaoDeArgumento()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new ArgumentException("Argumento inválido"));

            // Act
            var resultado = await PedidoUseCase.ConcluirPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Argumento inválido");
        }

        [Fact]
        public async Task ConcluirPreparacaoPedido_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.ConcluirPreparacaoPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region FinalizarPedido

        [Fact]
        public async Task FinalizarPedido_DeveRetornarErro_QuandoPedidoNaoForEncontrado()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync((Pedido?)null);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Pedido não localizado");
        }

        [Fact]
        public async Task FinalizarPedido_DeveFinalizarPedido_QuandoPedidoEncontrado()
        {
            // Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [
                    CriarPedidoItem(
                        CriarProdutoValido())
                ],
                StatusPedidoValido.Pronto);

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(true);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            pedido.Status.Valor.Should().Be(StatusPedidoValido.Finalizado);
        }

        [Fact]
        public async Task FinalizarPedido_DeveRetornarErro_QuandoAtualizacaoDeStatusFalhar()
        {
            // Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [
                    CriarPedidoItem(
                        CriarProdutoValido())
                ],
                StatusPedidoValido.Pronto);

            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ReturnsAsync(pedido);

            _pedidoGatewayMock.Setup(p => p.AtualizarStatusPedido(pedido))
                .ReturnsAsync(false);

            // Act
            var resultado = await PedidoUseCase.FinalizarPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Não foi possível finalizar o pedido");
        }

        [Fact]
        public async Task FinalizarPedido_DeveRetornarErroInterno_QuandoOcorrerExcecaoDeArgumento()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new ArgumentException("Argumento inválido"));

            // Act
            var resultado = await PedidoUseCase.FinalizarPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Argumento inválido");
        }

        [Fact]
        public async Task FinalizarPedido_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            _pedidoGatewayMock.Setup(p => p.PesquisarPorIdentificacao(It.IsAny<GuidValido>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await PedidoUseCase.FinalizarPedido(Guid.NewGuid().ToString(), _pedidoGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region Private Methods
        private static Cliente CriarClienteValido()
        {
            return new Cliente(
                Guid.NewGuid(),
                "nome",
                "email@email.com",
                "00000000191");
        }

        private static Produto CriarProdutoValido()
        {
            return new Produto(Guid.NewGuid(), 1);
        }

        private static PedidoItem CriarPedidoItem(Produto produto)
        {
            return new PedidoItem(
                Guid.NewGuid(),
                produto,
                1,
                "observacao");
        }

        private static Pedido CriarPedido(Cliente cliente, List<PedidoItem> itens, StatusPedidoValido status)
        {
            return new Pedido(
                Guid.NewGuid(),
                new DataPedidoValida(DateTime.Now),
                status,
                cliente,
                itens);
        }

        private static Pedido CriarPedidoValido()
        {
            return CriarPedido(
                CriarClienteValido(), 
                [CriarPedidoItem(
                    CriarProdutoValido()
                )], 
                new StatusPedidoValido(1)
            );
        }
        #endregion
    }
}