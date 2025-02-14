using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SnackTech.Orders.Common.CustomExceptions;
using SnackTech.Orders.Common.Dto.DataSource;
using SnackTech.Orders.Driver.DataBase.Context;
using SnackTech.Orders.Driver.DataBase.DataSources;
using SnackTech.Orders.Driver.DataBase.Entities;

namespace SnackTech.Orders.Driver.Database.Tests.DataSources
{
    public class ClienteDataSourceTest
    {
        #region InserirClienteAsync

        [Fact]
        public async Task InserirClienteAsync_DeveInserirClienteComSucesso()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();

            using var context = new RepositoryDbContext(options);
            var repository = new ClienteDataSource(context);

            var clienteDto = CriarClienteDtoValido();

            // Act
            var resultado = await repository.InserirClienteAsync(clienteDto);

            // Assert
            Assert.True(resultado);
            Assert.Equal(1, await context.Clientes.CountAsync()); // Verifica se foi salvo
        }

        [Fact]
        public async Task InserirClienteAsync_DeveLancarExcecao_SeClienteJaExiste()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new ClienteDataSource(context);

            var clienteDto = CriarClienteDtoValido();

            // Insere o cliente antes
            await repository.InserirClienteAsync(clienteDto);

            // Act
            var act = async () => await repository.InserirClienteAsync(clienteDto);

            // Assert
            await act.Should().ThrowAsync<ClienteRepositoryException>()
                .WithMessage("Já existe um cliente com o mesmo cpf e email no sistema.");
        }

        #endregion

        #region PesquisarPorCpfAsync

        [Fact]
        public async Task PesquisarPorCpfAsync_DeveRetornarCliente_QuandoCpfExiste()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new ClienteDataSource(context);

            var cliente = CriarClienteValido();

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            // Act
            var resultado = await repository.PesquisarPorCpfAsync(cliente.Cpf);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be(cliente.Nome);
            resultado.Email.Should().Be(cliente.Email);
            resultado.Cpf.Should().Be(cliente.Cpf);
        }

        [Fact]
        public async Task PesquisarPorCpfAsync_DeveRetornarNull_QuandoCpfNaoExiste()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new ClienteDataSource(context);

            // Act
            var resultado = await repository.PesquisarPorCpfAsync("00000000191");

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        #region PesquisarPorEmailAsync

        [Fact]
        public async Task PesquisarPorEmailAsync_DeveRetornarCliente_QuandoEmailExiste()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new ClienteDataSource(context);

            var cliente = CriarClienteValido();

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            // Act
            var resultado = await repository.PesquisarPorEmailAsync(cliente.Email);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be(cliente.Nome);
            resultado.Email.Should().Be(cliente.Email);
            resultado.Cpf.Should().Be(cliente.Cpf);
        }

        [Fact]
        public async Task PesquisarPorEmailAsync_DeveRetornarNull_QuandoEmailNaoExiste()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new ClienteDataSource(context);

            // Act
            var resultado = await repository.PesquisarPorEmailAsync("email@email.com");

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        #region PesquisarPorIdAsync

        [Fact]
        public async Task PesquisarPorIdAsync_DeveRetornarCliente_QuandoIdExiste()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new ClienteDataSource(context);

            var cliente = CriarClienteValido();

            context.Clientes.Add(cliente);
            await context.SaveChangesAsync();

            // Act
            var resultado = await repository.PesquisarPorIdAsync(cliente.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Nome.Should().Be(cliente.Nome);
            resultado.Email.Should().Be(cliente.Email);
            resultado.Cpf.Should().Be(cliente.Cpf);
        }

        [Fact]
        public async Task PesquisarPorIdAsync_DeveRetornarNull_QuandoIdNaoExiste()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new ClienteDataSource(context);

            // Act
            var resultado = await repository.PesquisarPorIdAsync(Guid.NewGuid());

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        #region Private Methods
        private static DbContextOptions<RepositoryDbContext> CriarOpcoesEmMemoria()
        {
            return new DbContextOptionsBuilder<RepositoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Garante um BD limpo a cada teste
                .Options;
        }

        private static ClienteDto CriarClienteDtoValido()
        {
            return new ClienteDto
            {
                Id = Guid.NewGuid(),
                Nome = "nome",
                Email = "email@email.com",
                Cpf = "00000000191"
            };
        }

        private static Cliente CriarClienteValido()
        {
            return new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = "nome",
                Email = "email@email.com",
                Cpf = "00000000191"
            };
        }
        #endregion
    }
}
