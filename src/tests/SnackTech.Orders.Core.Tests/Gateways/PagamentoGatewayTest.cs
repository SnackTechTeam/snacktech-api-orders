using FluentAssertions;
using Moq;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;
using SnackTech.Orders.Core.Gateways;

namespace SnackTech.Orders.Core.Tests.Gateways
{
    public class PagamentoGatewayTest
    {
        private readonly Mock<IPagamentoApi> _pagamentoApiMock;
        private readonly PagamentoGateway _pagamentoGateway;

        public PagamentoGatewayTest()
        {
            _pagamentoApiMock = new Mock<IPagamentoApi>();
            _pagamentoGateway = new PagamentoGateway(_pagamentoApiMock.Object);
        }

        [Fact]
        public async Task CriarPagamentoAsync_DeveRetornarSucesso_QuandoApiRetornaPagamentoValido()
        {
            // Arrange
            var pedido = CriarPedidoValido();
            var pagamentoEsperado = new PagamentoDto { QrCode = "123ABC", ValorTotal = 1 };
            var resultadoEsperado = new ResultadoOperacao<PagamentoDto>(pagamentoEsperado);            

            _pagamentoApiMock
                .Setup(api => api.CriarPagamentoAsync(It.IsAny<PedidoPagamentoDto>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _pagamentoGateway.CriarPagamentoAsync(pedido);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.QrCode.Should().Be("123ABC");
            _pagamentoApiMock.Verify(api => api.CriarPagamentoAsync(It.IsAny<PedidoPagamentoDto>()), Times.Once());
        }

        [Fact]
        public void ConverterItemParaDto_DeveConverterCorretamente()
        {
            // Arrange
            var pedidoItem = CriarPedidoItem(CriarProdutoValido());

            // Act
            var dto = PagamentoGateway.ConverterItemParaDto(pedidoItem);

            // Assert
            dto.Should().NotBeNull();
            dto.PedidoItemId.Should().Be(pedidoItem.Id.Valor);
            dto.Valor.Should().Be(pedidoItem.Valor());
        }

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
