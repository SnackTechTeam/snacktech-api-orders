using FluentAssertions;
using Moq;
using Refit;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Http.Refit;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Driver.Products.HttpApiClients;
using SnackTech.Orders.Driver.Products.Services;
using System.Net;

namespace SnackTech.Orders.Driver.Products.Tests.Services
{
    public class ProdutoApiTest
    {
        private readonly Mock<IProdutoHttpClient> _produtoHttpClientMock;
        private readonly Mock<IRequestExecutorHelper> _requestExecutorHelperMock;
        private readonly IProdutoApi _produtoApi;

        public ProdutoApiTest()
        {
            _produtoHttpClientMock = new Mock<IProdutoHttpClient>();
            _requestExecutorHelperMock = new Mock<IRequestExecutorHelper>();
            _produtoApi = new ProdutoApi(_produtoHttpClientMock.Object, _requestExecutorHelperMock.Object);
        }

        [Fact]
        public async Task BuscarProdutoAsync_QuandoProdutoExistir_DeveRetornarProduto()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var produtoDto = new ProdutoDto 
            { 
                IdentificacaoProduto = produtoId,
                Categoria = 1,
                Nome = "nome",
                Descricao = "descrição",
                Valor = 1
            };

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            var apiResponse = new ApiResponse<ProdutoDto>(responseMessage, produtoDto, new RefitSettings());

            _produtoHttpClientMock
                .Setup(x => x.BuscarProdutoAsync(produtoId))
                .ReturnsAsync(apiResponse);

            _requestExecutorHelperMock
                .Setup(helper => helper.Execute(It.IsAny<Func<Task<ApiResponse<ProdutoDto>>>>()))
                .Returns((Func<Task<ApiResponse<ProdutoDto>>> func) =>
                {
                    // Retorna o resultado esperado
                    return Task.FromResult(new ResultadoOperacao<ProdutoDto>(func().Result.Content));                    
                });

            // Act
            var result = await _produtoApi.BuscarProdutoAsync(produtoId);

            // Assert
            _produtoHttpClientMock.Verify(client => client.BuscarProdutoAsync(produtoId), Times.Once);
            _requestExecutorHelperMock.Verify(helper => helper.Execute(It.IsAny<Func<Task<ApiResponse<ProdutoDto>>>>()), Times.Once);

            result.Sucesso.Should().BeTrue();
            result.Dados.Should().NotBeNull();
            result.Dados.IdentificacaoProduto.Should().Be(produtoId);
        }
    }
}