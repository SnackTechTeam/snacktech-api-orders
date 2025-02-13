using FluentAssertions;
using Moq;
using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;
using SnackTech.Orders.Core.Interfaces;
using SnackTech.Orders.Core.UseCases;

namespace SnackTech.Orders.Core.Tests.UseCases
{
    public class ClienteUseCaseTest
    {
        private readonly Mock<IClienteGateway> _clienteGatewayMock;

        public ClienteUseCaseTest()
        {
            _clienteGatewayMock = new Mock<IClienteGateway>();
        }

        #region CriarNovoCliente

        [Fact]
        public async Task CriarNovoCliente_DeveRetornarSucesso_QuandoClienteForNovo()
        {
            // Arrange
            var clienteDto = new ClienteSemIdDto
            {
                Nome = "nome",
                Cpf = "00000000191",
                Email = "email@email.com"
            };
                
            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(clienteDto.Cpf))
                .ReturnsAsync((Cliente?)null);

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorEmail(clienteDto.Email))
                .ReturnsAsync((Cliente?)null);

            _clienteGatewayMock.Setup(g => g.CadastrarNovoCliente(It.IsAny<Cliente>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await ClienteUseCase.CriarNovoCliente(clienteDto, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Cpf.Should().Be(clienteDto.Cpf);
            resultado.Dados!.Email.Should().Be(clienteDto.Email);
        }

        [Fact]
        public async Task CriarNovoCliente_DeveRetornarErro_QuandoCpfJaExistir()
        {
            // Arrange
            var clienteExistente = CriarClienteValido();
            
            var clienteDto = new ClienteSemIdDto
            {
                Nome = clienteExistente.Nome,
                Cpf = clienteExistente.Cpf,
                Email = clienteExistente.Email
            };

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(It.IsAny<CpfValido>()))
                .ReturnsAsync(clienteExistente);

            // Act
            var resultado = await ClienteUseCase.CriarNovoCliente(clienteDto, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain($"Cliente com CPF {clienteDto.Cpf} já cadastrado.");
        }

        [Fact]
        public async Task CriarNovoCliente_DeveRetornarErro_QuandoEmailJaExistir()
        {
            // Arrange
            var clienteExistente = CriarClienteValido();

            var clienteDto = new ClienteSemIdDto
            {
                Nome = clienteExistente.Nome,
                Cpf = clienteExistente.Cpf,
                Email = clienteExistente.Email
            };

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(It.IsAny<CpfValido>()))
                .ReturnsAsync((Cliente?)null);

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorEmail(It.IsAny<EmailValido>()))
                .ReturnsAsync(clienteExistente);

            // Act
            var resultado = await ClienteUseCase.CriarNovoCliente(clienteDto, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain($"Cliente com email {clienteDto.Email} já cadastrado.");
        }

        [Fact]
        public async Task CriarNovoCliente_DeveRetornarErro_QuandoCadastroFalhar()
        {            
            // Arrange
            var clienteDto = new ClienteSemIdDto
            {
                Nome = "nome",
                Cpf = "00000000191",
                Email = "email@email.com"
            };

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(It.IsAny<CpfValido>()))
                .ReturnsAsync((Cliente?)null);

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorEmail(It.IsAny<EmailValido>()))
                .ReturnsAsync((Cliente?)null);

            _clienteGatewayMock.Setup(g => g.CadastrarNovoCliente(It.IsAny<Cliente>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await ClienteUseCase.CriarNovoCliente(clienteDto, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain($"Não foi possível cadastrar cliente {clienteDto.Nome}.");
        }

        [Fact]
        public async Task CriarNovoCliente_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            var clienteDto = new ClienteSemIdDto
            {
                Nome = "nome",
                Cpf = "00000000191",
                Email = "email@email.com"
            };

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(It.IsAny<CpfValido>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await ClienteUseCase.CriarNovoCliente(clienteDto, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region PesquisarPorCpf

        [Fact]
        public async Task PesquisarPorCpf_DeveRetornarCliente_QuandoEncontrado()
        {
            // Arrange
            var cpf = "00000000191";
            var clienteEsperado = CriarClienteValido();

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(It.IsAny<CpfValido>()))
                .ReturnsAsync(clienteEsperado);

            // Act
            var resultado = await ClienteUseCase.PesquisarPorCpf(cpf, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Cpf.Should().Be(cpf);
            resultado.Dados!.Nome.Should().Be(clienteEsperado.Nome);
            resultado.Dados!.Email.Should().Be(clienteEsperado.Email);
        }

        [Fact]
        public async Task PesquisarPorCpf_DeveRetornarErro_QuandoClienteNaoForEncontrado()
        {
            // Arrange
            var cpf = "00000000191";

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(It.IsAny<CpfValido>()))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await ClienteUseCase.PesquisarPorCpf(cpf, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain($"Não foi possível encontrar um cliente com CPF {cpf}.");
        }

        [Fact]
        public async Task PesquisarPorCpf_DeveRetornarErroInterno_QuandoOcorreExcecao()
        {
            // Arrange
            var cpf = "00000000191";

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(It.IsAny<CpfValido>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await ClienteUseCase.PesquisarPorCpf(cpf, _clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Erro inesperado");
        }

        #endregion

        #region SelecionarClientePadrao

        [Fact]
        public async Task SelecionarClientePadrao_DeveRetornarCliente_QuandoEncontrado()
        {
            // Arrange
            var clientePadrao = new Cliente(Guid.NewGuid(), "Cliente Padrão", "cliente@email.com", Cliente.CPF_CLIENTE_PADRAO);

            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(Cliente.CPF_CLIENTE_PADRAO))
                .ReturnsAsync(clientePadrao);

            // Act
            var resultado = await ClienteUseCase.SelecionarClientePadrao(_clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados!.Cpf.Should().Be(Cliente.CPF_CLIENTE_PADRAO);
            resultado.Dados!.Nome.Should().Be(clientePadrao.Nome);
            resultado.Dados!.Email.Should().Be(clientePadrao.Email);
        }

        [Fact]
        public async Task SelecionarClientePadrao_DeveRetornarErro_QuandoClienteNaoForEncontrado()
        {
            // Arrange
            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(Cliente.CPF_CLIENTE_PADRAO))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await ClienteUseCase.SelecionarClientePadrao(_clienteGatewayMock.Object);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Dados.Should().BeNull();
            resultado.Mensagem.Should().Contain("Cliente padrão não encontrado.");
        }

        [Fact]
        public async Task SelecionarClientePadrao_DeveRetornarErroInterno_QuandoOcorrerExcecao()
        {
            // Arrange
            _clienteGatewayMock.Setup(g => g.ProcurarClientePorCpf(Cliente.CPF_CLIENTE_PADRAO))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var resultado = await ClienteUseCase.SelecionarClientePadrao(_clienteGatewayMock.Object);

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
        #endregion
    }
}
