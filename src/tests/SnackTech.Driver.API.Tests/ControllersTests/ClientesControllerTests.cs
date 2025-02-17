using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Core.Interfaces;
using SnackTech.Orders.Driver.API.Controllers;
using SnackTech.Orders.Driver.API.CustomResponses;

namespace SnackTech.Orders.Driver.API.Tests.ControllersTests
{
    public class ClientesControllerTests
    {
        private readonly Mock<ILogger<ClientesController>> logger;
        private readonly Mock<IClienteController> _clienteController;
        private readonly ClientesController _clientesController;
        
        public ClientesControllerTests()
        {
            logger = new Mock<ILogger<ClientesController>>();
            _clienteController = new Mock<IClienteController>();
            _clientesController = new ClientesController(logger.Object, _clienteController.Object);
        }

        #region Post

        [Fact]
        public async Task PostWithSuccess()
        {
            //Arrange
            var clienteId = Guid.NewGuid();
            var cadastroCliente = new ClienteSemIdDto
            {
                Cpf = "00000000191",
                Email = "email@email.com",
                Nome = "nome"
            };

            var retornoCliente = new ClienteDto 
            { 
                IdentificacaoCliente = clienteId,
                Cpf = cadastroCliente.Cpf,
                Email = cadastroCliente.Email,
                Nome = cadastroCliente.Nome
            };


            _clienteController.Setup(c => c.CadastrarNovoCliente(It.IsAny<ClienteSemIdDto>()))
                            .ReturnsAsync(new ResultadoOperacao<ClienteDto>(retornoCliente));

            //Act
            var resultado = await _clientesController.Post(cadastroCliente);

            //Assert
            Assert.IsType<OkObjectResult>(resultado);           
        }

        [Fact]
        public async Task PostWithBadRequest()
        {
            //Arrange
            var cadastroCliente = new ClienteSemIdDto
            {
                Cpf = "00000000191",
                Email = "email@email.com",
                Nome = "nome"
            };

            _clienteController.Setup(c => c.CadastrarNovoCliente(It.IsAny<ClienteSemIdDto>()))
                            .ReturnsAsync(new ResultadoOperacao<ClienteDto>("Erro de lógica", true));

            //Act
            var resultado = await _clientesController.Post(cadastroCliente);

            //Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task PostWithInternalServerError()
        {
            //Arrange
            var cadastroCliente = new ClienteSemIdDto
            {
                Cpf = "00000000191",
                Email = "email@email.com",
                Nome = "nome"
            };

            _clienteController.Setup(c => c.CadastrarNovoCliente(It.IsAny<ClienteSemIdDto>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            //Act
            var resultado = await _clientesController.Post(cadastroCliente);

            //Assert
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region GetByCpfWithBadRequest

        [Fact]
        public async Task GetByCpfWithSuccess()
        {
            var cpf = "00000000191";
            var retornoCliente = new ClienteDto { };
            _clienteController.Setup(c => c.IdentificarPorCpf(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<ClienteDto>(retornoCliente));

            var resultado = await _clientesController.GetByCpf(cpf);

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task GetByCpfWithBadRequest()
        {
            var cpf = "00000000191";
            _clienteController.Setup(c => c.IdentificarPorCpf(It.IsAny<string>()))
                            .ReturnsAsync(new ResultadoOperacao<ClienteDto>("Erro de lógica", true));

            var resultado = await _clientesController.GetByCpf(cpf);

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task GetByCpfWithInternalServerError()
        {
            var cpf = "00000000191";
            _clienteController.Setup(c => c.IdentificarPorCpf(It.IsAny<string>()))
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var resultado = await _clientesController.GetByCpf(cpf);
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion

        #region GetDefaultClientWithSuccess

        [Fact]
        public async Task GetDefaultClientWithSuccess()
        {
            var retornoCliente = new ClienteDto { };
            _clienteController.Setup(c => c.SelecionarClientePadrao())
                            .ReturnsAsync(new ResultadoOperacao<ClienteDto>(retornoCliente));

            var resultado = await _clientesController.GetDefaultClient();

            Assert.IsType<OkObjectResult>(resultado);
        }

        [Fact]
        public async Task GetDefaultClientWithBadRequest()
        {

            _clienteController.Setup(c => c.SelecionarClientePadrao())
                            .ReturnsAsync(new ResultadoOperacao<ClienteDto>("Erro de lógica", true));

            var resultado = await _clientesController.GetDefaultClient();

            var objectResult = Assert.IsType<BadRequestObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Null(payload.Exception);
            Assert.Equal("Erro de lógica", payload.Message);
        }

        [Fact]
        public async Task GetDefaultClientWithInternalServerError()
        {
            _clienteController.Setup(c => c.SelecionarClientePadrao())
                            .ThrowsAsync(new Exception("Erro inesperado"));

            var resultado = await _clientesController.GetDefaultClient();
            var objectResult = Assert.IsType<ObjectResult>(resultado);
            var payload = Assert.IsType<ErrorResponse>(objectResult.Value);

            Assert.NotNull(payload.Exception);
            Assert.Equal("Erro inesperado", payload.Message);
        }

        #endregion
    }
}