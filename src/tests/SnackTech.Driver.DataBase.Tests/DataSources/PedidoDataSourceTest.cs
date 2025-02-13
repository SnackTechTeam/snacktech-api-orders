using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using SnackTech.Orders.Common.CustomExceptions;
using SnackTech.Orders.Common.Dto.DataSource;
using SnackTech.Orders.Common.Enums;
using SnackTech.Orders.Driver.DataBase.Context;
using SnackTech.Orders.Driver.DataBase.DataSources;
using SnackTech.Orders.Driver.DataBase.Entities;

namespace SnackTech.Orders.Driver.Database.Tests.DataSources
{
    public class PedidoDataSourceTest
    {
        #region AlterarItensDoPedidoAsync

        [Fact]
        public async Task AlterarItensDoPedidoAsync_DeveAtualizarItens_QuandoPedidoExiste()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                Itens =
                [
                    new PedidoItem { Id = Guid.NewGuid(), Quantidade = 1, ValorTotal = 10.00m, Observacao = "" }
                ]
            };

            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            var pedidoDto = new PedidoDto
            {
                Id = pedido.Id,
                Itens =
                [
                    new PedidoItemDto { Id = pedido.Itens[0].Id, Quantidade = 2, ValorTotal = 20.00m, Observacao = "" }
                ]
            };

            // Act
            var resultado = await repository.AlterarItensDoPedidoAsync(pedidoDto);

            // Assert
            resultado.Should().BeTrue();
            var pedidoAtualizado = await context.Pedidos.Include(p => p.Itens).FirstAsync(p => p.Id == pedido.Id);
            pedidoAtualizado.Itens.Should().ContainSingle();
            pedidoAtualizado.Itens[0].Quantidade.Should().Be(2);
            pedidoAtualizado.Itens[0].ValorTotal.Should().Be(20.00m);
        }

        [Fact]
        public async Task AlterarItensDoPedidoAsync_DeveAdicionarNovoItem_QuandoNaoExisteNoPedido()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var pedido = new Pedido { Id = Guid.NewGuid(), Itens = [] };

            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            var novoItem = new PedidoItemDto { Id = Guid.NewGuid(), Quantidade = 3, ValorTotal = 30.00m, Observacao = "" };

            var pedidoDto = new PedidoDto { Id = pedido.Id, Itens = [novoItem] };

            // Act
            var resultado = await repository.AlterarItensDoPedidoAsync(pedidoDto);

            // Assert
            resultado.Should().BeTrue();
            var pedidoAtualizado = await context.Pedidos.Include(p => p.Itens).FirstAsync(p => p.Id == pedido.Id);
            pedidoAtualizado.Itens.Should().HaveCount(1);
            pedidoAtualizado.Itens[0].Quantidade.Should().Be(3);
            pedidoAtualizado.Itens[0].ValorTotal.Should().Be(30.00m);
        }

        [Fact]
        public async Task AlterarItensDoPedidoAsync_DeveRemoverItem_QuandoNaoEstaMaisNaLista()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var itemRemovido = new PedidoItem { Id = Guid.NewGuid(), Quantidade = 1, ValorTotal = 10.00m, Observacao = "" };
            var pedido = new Pedido { Id = Guid.NewGuid(), Itens = [itemRemovido] };

            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            var pedidoDto = new PedidoDto { Id = pedido.Id, Itens = [] }; // Nenhum item

            // Act
            var resultado = await repository.AlterarItensDoPedidoAsync(pedidoDto);

            // Assert
            resultado.Should().BeTrue();
            var pedidoAtualizado = await context.Pedidos.Include(p => p.Itens).FirstAsync(p => p.Id == pedido.Id);
            pedidoAtualizado.Itens.Should().BeEmpty();
        }

        [Fact]
        public async Task AlterarItensDoPedidoAsync_DeveLancarExcecao_QuandoPedidoNaoEncontrado()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var pedidoDto = new PedidoDto { Id = Guid.NewGuid(), Itens = [] };

            // Act
            Func<Task> act = async () => await repository.AlterarItensDoPedidoAsync(pedidoDto);

            // Assert
            await act.Should().ThrowAsync<PedidoRepositoryException>()
                .WithMessage($"Pedido com identificacao {pedidoDto.Id} não encontrado no banco de dados.");
        }

        #endregion

        #region AtualizarStatusPedidoAsync

        [Fact]
        public async Task AtualizarStatusPedidoAsync_DeveAtualizarStatusSePedidoExistir()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var pedidoExistente = new Pedido { Id = Guid.NewGuid(), Status = StatusPedido.Recebido };
            await context.Pedidos.AddAsync(pedidoExistente);
            await context.SaveChangesAsync();

            var pedidoDto = new PedidoDto { Id = pedidoExistente.Id, Status = (int)StatusPedido.EmPreparacao };

            // Act
            var resultado = await repository.AtualizarStatusPedidoAsync(pedidoDto);
            var pedidoAtualizado = await context.Pedidos.FindAsync(pedidoExistente.Id);

            // Assert
            resultado.Should().BeTrue();
            pedidoAtualizado.Should().NotBeNull();
            pedidoAtualizado.Status.Should().Be(StatusPedido.EmPreparacao);
        }

        [Fact]
        public async Task AtualizarStatusPedidoAsync_DeveRetornarFalsoSePedidoNaoExistir()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var pedidoDto = new PedidoDto { Id = Guid.NewGuid(), Status = (int)StatusPedido.EmPreparacao };

            // Act
            var resultado = await repository.AtualizarStatusPedidoAsync(pedidoDto);

            // Assert
            resultado.Should().BeFalse();
        }

        #endregion

        #region InserirPedidoAsync

        [Fact]
        public async Task InserirPedidoAsync_DeveRetornarTrue_SeInsercaoForBemSucedida()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente Teste" };
            await context.Clientes.AddAsync(cliente);
            await context.SaveChangesAsync();

            var pedidoDto = new PedidoDto
            {
                Id = Guid.NewGuid(),
                Cliente = new ClienteDto { Id = cliente.Id, Nome = cliente.Nome },
                Itens = new List<PedidoItemDto>()
            };

            // Act
            var resultado = await repository.InserirPedidoAsync(pedidoDto);           

            // Assert
            resultado.Should().BeTrue();
            (await context.Pedidos.CountAsync()).Should().Be(1);
        }

        #endregion

        #region Private Methods

        private static DbContextOptions<RepositoryDbContext> CriarOpcoesEmMemoria()
        {
            return new DbContextOptionsBuilder<RepositoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Garante um BD limpo a cada teste
                .Options;
        }

        #endregion
    }
}
