using FluentAssertions;
using Moq;
using Refit;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Http.Refit;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Driver.Payments.HttpApiClients;
using SnackTech.Orders.Driver.Payments.Services;
using System.Net;

namespace SnackTech.Orders.Driver.Payments.Tests.Services
{
    public class PagamentoApiTest
    {
        private readonly Mock<IPagamentoHttpClient> _pagamentoHttpClientMock;
        private readonly Mock<IRequestExecutorHelper> _requestExecutorHelperMock;
        private readonly IPagamentoApi _pagamentoApi;

        public PagamentoApiTest()
        {
            _pagamentoHttpClientMock = new Mock<IPagamentoHttpClient>();
            _requestExecutorHelperMock = new Mock<IRequestExecutorHelper>();
            _pagamentoApi = new PagamentoApi(_pagamentoHttpClientMock.Object, _requestExecutorHelperMock.Object);
        }

        [Fact]
        public async Task CriarPagamentoAsync_QuandoChamado_DeveRetornarPagamentoCriado()
        {
            // Arrange
            var pagamentoId = Guid.NewGuid();
            var pedidoPagamentoDto = new PedidoPagamentoDto
            {
                PedidoId = pagamentoId,
                Cliente = new()
                {
                    Id = Guid.NewGuid(),
                    Email = "email@email.com",
                    Nome = "nome"
                },
                Itens = 
                [
                    new PedidoItemPagamentoDto
                    {
                        PedidoItemId = Guid.NewGuid(),
                        Valor = 1
                    }
                ]
            };

            var pagamentoDto = new PagamentoDto
            {
                Id = pagamentoId,
                QrCode = "123",
                ValorTotal = 1
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            var apiResponse = new ApiResponse<PagamentoDto>(responseMessage, pagamentoDto, new RefitSettings());

            _pagamentoHttpClientMock
                .Setup(x => x.CriarPagamentoAsync(pedidoPagamentoDto))
                .ReturnsAsync(apiResponse);

            _requestExecutorHelperMock
                .Setup(helper => helper.Execute(It.IsAny<Func<Task<ApiResponse<PagamentoDto>>>>()))
                .Returns((Func<Task<ApiResponse<PagamentoDto>>> func) =>
                {
                    // Retorna o resultado esperado
                    return Task.FromResult(new ResultadoOperacao<PagamentoDto>(func().Result.Content));
                });

            // Act
            var result = await _pagamentoApi.CriarPagamentoAsync(pedidoPagamentoDto);

            // Assert
            _pagamentoHttpClientMock.Verify(client => client.CriarPagamentoAsync(pedidoPagamentoDto), Times.Once);

            result.Sucesso.Should().BeTrue();
            result.Dados.Should().NotBeNull();
            result.Dados.Id.Should().Be(pagamentoId);
            result.Dados.QrCode.Should().Be("123");
            result.Dados.ValorTotal.Should().Be(1);
        }

        [Fact]
        public async Task CriarPagamentoAsync_QuandoApiRetornaErro_DeveRetornarResultadoComErro()
        {
            // Arrange
            var pagamentoId = Guid.NewGuid();
            var pedidoPagamentoDto = new PedidoPagamentoDto
            {
                PedidoId = pagamentoId,
                Cliente = new()
                {
                    Id = Guid.NewGuid(),
                    Email = "email@email.com",
                    Nome = "nome"
                },
                Itens =
                [
                    new PedidoItemPagamentoDto
                    {
                        PedidoItemId = Guid.NewGuid(),
                        Valor = 1
                    }
                ]
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var apiResponse = new ApiResponse<PagamentoDto>(responseMessage, null, new RefitSettings());

            _pagamentoHttpClientMock
                .Setup(x => x.CriarPagamentoAsync(pedidoPagamentoDto))
                .ReturnsAsync(apiResponse);

            _requestExecutorHelperMock
                .Setup(helper => helper.Execute(It.IsAny<Func<Task<ApiResponse<PagamentoDto>>>>()))
                .Returns((Func<Task<ApiResponse<PagamentoDto>>> func) =>
                {
                    return Task.FromResult(new ResultadoOperacao<PagamentoDto>("Erro ao processar pagamento", true));
                });

            // Act
            var result = await _pagamentoApi.CriarPagamentoAsync(pedidoPagamentoDto);

            // Assert
            result.Sucesso.Should().BeFalse();
            result.Mensagem.Should().Contain("Erro ao processar pagamento");
        }
    }
}
