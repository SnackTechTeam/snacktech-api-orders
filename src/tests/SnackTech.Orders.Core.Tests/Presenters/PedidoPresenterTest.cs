using FluentAssertions;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;
using SnackTech.Orders.Core.Presenters;

namespace SnackTech.Orders.Core.Tests.Presenters
{
    public class PedidoPresenterTest
    {

        [Fact]
        public void ApresentarResultadoPedido_DeveRetornarResultadoOperacaoComPedidoRetornoDto()
        {
            //Arrange
            var pedido = CriarPedidoCompleto();

            // Act
            var resultado = PedidoPresenter.ApresentarResultadoPedido(pedido);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.IdentificacaoPedido.Should().Be(pedido.Id.Valor);
            resultado.Dados.DataCriacao.Should().Be(pedido.DataCriacao.Valor);
            resultado.Dados.Status.Should().Be(pedido.Status.Valor);
            resultado.Dados.Cliente.Cpf.Should().Be(pedido.Cliente.Cpf.Valor);
            resultado.Dados.Itens.Count().Should().Be(pedido.Itens.Count());
        }

        [Fact]
        public void ApresentarResultadoPedido_DeveRetornarResultadoOperacaoComListaPedidoRetornoDto()
        {
            //Arrange
            IEnumerable<Pedido> pedidos =
            [
                CriarPedidoCompleto(),
                CriarPedidoCompleto()
            ];               

            // Act
            var resultado = PedidoPresenter.ApresentarResultadoPedido(pedidos);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Count().Should().Be(pedidos.Count());
        }

        [Fact]
        public void ApresentarResultadoPedidoIniciado_DeveRetornarResultadoOperacaoComGuid()
        {
            //Arrange
            var pedido = CriarPedidoCompleto();

            // Act
            var resultado = PedidoPresenter.ApresentarResultadoPedidoIniciado(pedido);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Dados.Should().NotBeEmpty();
            resultado.Dados.Should().Be(pedido.Id.Valor);
        }

        [Fact]
        public void ApresentarResultadoPedido_DeveRetornarResultadoOperacaoComPagamentoDto()
        {
            //Arrange
            var pedido = CriarPedidoCompleto();
            var dadoPagamento = "abcd";

            // Act
            var resultado = PedidoPresenter.ApresentarResultadoPedido(pedido, dadoPagamento);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Id.Should().Be(pedido.Id.Valor);
            resultado.Dados.ValorTotal.Should().Be(pedido.ValorTotal.Valor);
            resultado.Dados.QrCode.Should().Be(dadoPagamento);
        }

        [Fact]
        public void ApresentarResultadoOk_DeveRetornarResultadoOperacao()
        {
            //Act
            var resultado = PedidoPresenter.ApresentarResultadoOk();

            // Assert
            resultado.Should().NotBeNull();
        }

        private Pedido CriarPedidoCompleto()
        {
            var id = Guid.NewGuid();
            var dataCriacao = new DataPedidoValida(DateTime.Now);
            var status = new StatusPedidoValido(1);
            var cliente = new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191");
            var itens = new List<PedidoItem> { new PedidoItem(Guid.NewGuid(), new Produto(Guid.NewGuid(), 1), 1, "observacao") };

            return new Pedido(id, dataCriacao, status, cliente, itens);
        }
    }
}
