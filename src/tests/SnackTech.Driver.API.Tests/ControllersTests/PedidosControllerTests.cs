using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Core.Interfaces;
using SnackTech.Orders.Driver.API.Controllers;
using SnackTech.Orders.Driver.API.CustomResponses;

namespace SnackTech.Orders.Driver.API.Tests.ControllersTests
{
    public class PedidosControllerTests
    {
        private readonly Mock<ILogger<PedidosController>> _logger;
        private readonly Mock<IPedidoController> _pedidoController;
        private readonly PedidosController pedidosController;
        
        public PedidosControllerTests()
        {
            _logger = new Mock<ILogger<PedidosController>>();
            _pedidoController = new Mock<IPedidoController>();
            pedidosController = new PedidosController(_logger.Object, _pedidoController.Object);
        }

        #region IniciarPedido

        [Fact]
        public async Task IniciarPedidoWithSuccess()
        {
            _pedidoController.Setup(c => c.IniciarPedido(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<Guid>(Guid.NewGuid()));

            var iniciarPedido = new IniciarPedidoDto
            {
                Cpf = "1"
            };
            var resultado = await pedidosController.IniciarPedido(iniciarPedido);

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task IniciarPedidoWithBadRequest()
        {
            _pedidoController.Setup(c => c.IniciarPedido(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<Guid>("Erro de lógica", true));

            var iniciarPedido = new IniciarPedidoDto
            {
                Cpf = "1"
            };
            var resultado = await pedidosController.IniciarPedido(iniciarPedido);

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task IniciarPedidoWithInternalServerError()
        {
            _pedidoController.Setup(c => c.IniciarPedido(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var iniciarPedido = new IniciarPedidoDto
            {
                Cpf = "1"
            };
            var resultado = await pedidosController.IniciarPedido(iniciarPedido);
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);            
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region AtualizarPedido

        [Fact]
        public async Task AtualizarPedidoWithSuccess()
        {
            var pedidoRetornoDto = new PedidoRetornoDto
            {
                IdentificacaoPedido = Guid.NewGuid()
            };

            _pedidoController.Setup(c => c.AtualizarPedido(It.IsAny<PedidoAtualizacaoDto>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>(pedidoRetornoDto));

            var atualizacaoPedido = new PedidoAtualizacaoDto() { PedidoItens = [] };

            var resultado = await pedidosController.AtualizarPedido(atualizacaoPedido);

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task AtualizarPedidoWithBadRequest()
        {
            _pedidoController.Setup(c => c.AtualizarPedido(It.IsAny<PedidoAtualizacaoDto>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>("Erro de lógica", true));

            var atualizacaoPedido = new PedidoAtualizacaoDto() { PedidoItens = [] };

            var resultado = await pedidosController.AtualizarPedido(atualizacaoPedido);

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task AtualizarPedidoWithInternalServerError()
        {
            _pedidoController.Setup(c => c.AtualizarPedido(It.IsAny<PedidoAtualizacaoDto>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var atualizacaoPedido = new PedidoAtualizacaoDto() { PedidoItens = [] };

            var resultado = await pedidosController.AtualizarPedido(atualizacaoPedido);
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region FinalizarPedidoParaPagamento

        [Fact]
        public async Task FinalizarPedidoParaPagamentoWithSuccess()
        {
            var pedidoId = Guid.NewGuid();
            var pagamentoDto = new PagamentoDto()
            {
                Id = pedidoId,
                QrCode = "123",
                ValorTotal = 1
            };

            _pedidoController.Setup(c => c.FinalizarPedidoParaPagamento(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PagamentoDto>(pagamentoDto));

            var resultado = await pedidosController.FinalizarPedidoParaPagamento(pedidoId.ToString());

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamentoWithBadRequest()
        {
            _pedidoController.Setup(c => c.FinalizarPedidoParaPagamento(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PagamentoDto>("Erro de lógica", true));

            var resultado = await pedidosController.FinalizarPedidoParaPagamento(Guid.NewGuid().ToString());

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task FinalizarPedidoParaPagamentoWithInternalServerError()
        {
            _pedidoController.Setup(c => c.FinalizarPedidoParaPagamento(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var resultado = await pedidosController.FinalizarPedidoParaPagamento(Guid.NewGuid().ToString());
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region ListarPedidosParaPagamento

        [Fact]
        public async Task ListarPedidosParaPagamentoWithSuccess()
        {
            _pedidoController.Setup(p => p.ListarPedidosParaPagamento())
                        .ReturnsAsync(new ResultadoOperacao<IEnumerable<PedidoRetornoDto>>([
                                    new PedidoRetornoDto { Itens = [] },
                                    new PedidoRetornoDto { Itens = [] },
                                    new PedidoRetornoDto { Itens = [] },
                                    new PedidoRetornoDto { Itens = [] }
                        ]));

            var resultado = await pedidosController.ListarPedidosParaPagamento();

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task ListarPedidosParaPagamentoWithSuccessButEmptyList()
        {
            _pedidoController.Setup(p => p.ListarPedidosParaPagamento())
                        .ReturnsAsync(new ResultadoOperacao<IEnumerable<PedidoRetornoDto>>([]));

            var resultado = await pedidosController.ListarPedidosParaPagamento();

            Assert.IsType<OkObjectResult>(resultado);
        }

        #endregion

        #region BuscarPorIdenticacao

        [Fact]
        public async Task BuscarPorIdenticacaoWithSuccess()
        {
            var retornoPedido = new PedidoRetornoDto { Itens = [] };
            _pedidoController.Setup(c => c.BuscarPorIdenticacao(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>(retornoPedido));

            var resultado = await pedidosController.BuscarPorIdenticacao(Guid.NewGuid().ToString());

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task BuscarPorIdenticacaoWithBadRequest()
        {
            var cpf = "00000000191";
            _pedidoController.Setup(c => c.BuscarPorIdenticacao(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>("Erro de lógica", true));

            var resultado = await pedidosController.BuscarPorIdenticacao(cpf);

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task BuscarPorIdenticacaoWithInternalServerError()
        {
            var cpf = "00000000191";
            _pedidoController.Setup(c => c.BuscarPorIdenticacao(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var resultado = await pedidosController.BuscarPorIdenticacao(cpf);
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region BuscarUltimoPedidoCliente

        [Fact]
        public async Task BuscarUltimoPedidoClienteWithSuccess()
        {
            var retornoPedido = new PedidoRetornoDto { Itens = [] };
            _pedidoController.Setup(c => c.BuscarUltimoPedidoCliente(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>(retornoPedido));

            var resultado = await pedidosController.BuscarUltimoPedidoCliente("11");

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task BuscarUltimoPedidoClienteWithBadRequest()
        {
            var cpf = "00000000191";
            _pedidoController.Setup(c => c.BuscarUltimoPedidoCliente(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>("Erro de lógica", true));

            var resultado = await pedidosController.BuscarUltimoPedidoCliente(cpf);

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task BuscarUltimoPedidoClienteWithInternalServerError()
        {
            var cpf = "00000000191";
            _pedidoController.Setup(c => c.BuscarUltimoPedidoCliente(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var resultado = await pedidosController.BuscarUltimoPedidoCliente(cpf);
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region ListarPedidosAtivos

        [Fact]
        public async Task ListarPedidosAtivosWithSuccess()
        {
            _pedidoController.Setup(p => p.ListarPedidosAtivos())
                        .ReturnsAsync(new ResultadoOperacao<IEnumerable<PedidoRetornoDto>>([
                                    new PedidoRetornoDto { Itens = [] },
                                    new PedidoRetornoDto { Itens = [] },
                                    new PedidoRetornoDto { Itens = [] },
                                    new PedidoRetornoDto { Itens = [] }
                        ]));

            var resultado = await pedidosController.ListarPedidosAtivos();

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task ListarPedidosAtivosWithSuccessButEmptyList()
        {
            _pedidoController.Setup(p => p.ListarPedidosAtivos())
                        .ReturnsAsync(new ResultadoOperacao<IEnumerable<PedidoRetornoDto>>([]));

            var resultado = await pedidosController.ListarPedidosAtivos();

            Assert.IsType<OkObjectResult>(resultado);
        }

        #endregion

        #region IniciarPreparacaoPedido

        [Fact]
        public async Task IniciarPreparacaoPedidoWithSuccess()
        {
            _pedidoController.Setup(c => c.IniciarPreparacaoPedido(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao());

            var resultado = await pedidosController.IniciarPreparacaoPedido(Guid.NewGuid().ToString());

            Assert.IsType<OkResult>(resultado);
        }

        [Fact]
        public async Task IniciarPreparacaoPedidoWithBadRequest()
        {
            _pedidoController.Setup(c => c.IniciarPreparacaoPedido(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>("Erro de lógica", true));

            var resultado = await pedidosController.IniciarPreparacaoPedido(Guid.NewGuid().ToString());

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task IniciarPreparacaoPedidoWithInternalServerError()
        {
            _pedidoController.Setup(c => c.IniciarPreparacaoPedido(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var resultado = await pedidosController.IniciarPreparacaoPedido(Guid.NewGuid().ToString());
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region ConcluirPreparacaoPedido

        [Fact]
        public async Task ConcluirPreparacaoPedidoWithSuccess()
        {
            _pedidoController.Setup(c => c.ConcluirPreparacaoPedido(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao());

            var resultado = await pedidosController.ConcluirPreparacaoPedido(Guid.NewGuid().ToString());

            Assert.IsType<OkResult>(resultado);
        }

        [Fact]
        public async Task ConcluirPreparacaoPedidoWithBadRequest()
        {
            _pedidoController.Setup(c => c.ConcluirPreparacaoPedido(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>("Erro de lógica", true));

            var resultado = await pedidosController.ConcluirPreparacaoPedido(Guid.NewGuid().ToString());

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task ConcluirPreparacaoPedidoWithInternalServerError()
        {
            _pedidoController.Setup(c => c.ConcluirPreparacaoPedido(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var resultado = await pedidosController.ConcluirPreparacaoPedido(Guid.NewGuid().ToString());
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region FinalizarPedido

        [Fact]
        public async Task FinalizarPedidoWithSuccess()
        {
            _pedidoController.Setup(c => c.FinalizarPedido(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao());

            var resultado = await pedidosController.FinalizarPedido(Guid.NewGuid().ToString());

            Assert.IsType<OkResult>(resultado);
        }

        [Fact]
        public async Task FinalizarPedidoWithBadRequest()
        {
            _pedidoController.Setup(c => c.FinalizarPedido(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<PedidoRetornoDto>("Erro de lógica", true));

            var resultado = await pedidosController.FinalizarPedido(Guid.NewGuid().ToString());

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task FinalizarPedidoWithInternalServerError()
        {
            _pedidoController.Setup(c => c.FinalizarPedido(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var resultado = await pedidosController.FinalizarPedido(Guid.NewGuid().ToString());
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion
    }
}