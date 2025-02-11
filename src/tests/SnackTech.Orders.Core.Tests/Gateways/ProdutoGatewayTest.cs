using FluentAssertions;
using Moq;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Core.Gateways;

namespace SnackTech.Orders.Core.Tests.Gateways;

public class ProdutoGatewayTest
{
    private readonly Mock<IProdutoApi> _produtoApiMock;
    private readonly ProdutoGateway _produtoGateway;

    public ProdutoGatewayTest()
    {
        _produtoApiMock = new Mock<IProdutoApi>();
        _produtoGateway = new ProdutoGateway(_produtoApiMock.Object);
    }

    [Fact]
    public async Task BuscarProdutoAsync_DeveRetornarSucesso_QuandoApiRetornaProdutoValido()
    {
        // Arrange
        var produtoId = Guid.NewGuid();
        var produtoEsperado = new ProdutoDto
        {
            IdentificacaoProduto = produtoId,
            Valor = 1
        };
        var resultadoEsperado = new ResultadoOperacao<ProdutoDto>(produtoEsperado);

        _produtoApiMock
            .Setup(api => api.BuscarProdutoAsync(It.IsAny<Guid>()))
            .ReturnsAsync(resultadoEsperado);

        // Act
        var resultado = await _produtoGateway.BuscarProdutoAsync(produtoId);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Dados.Should().NotBeNull();
        resultado.Dados.Valor.Should().Be(1);
        _produtoApiMock.Verify(api => api.BuscarProdutoAsync(It.IsAny<Guid>()), Times.Once());
    }
}
