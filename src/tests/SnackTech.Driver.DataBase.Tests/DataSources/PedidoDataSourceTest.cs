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

            var pedido = CriarPedidoValido(
                [CriarPedidoItemValido()], 
                CriarClienteValido()
            );

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

            var pedido = CriarPedidoValido(
                [CriarPedidoItemValido()],
                CriarClienteValido()
            );

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
            var pedido = CriarPedidoValido(
                [itemRemovido],
                CriarClienteValido()
            );

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

            var pedidoExistente = CriarPedidoValido(
                [CriarPedidoItemValido()],
                CriarClienteValido()
            );

            await context.Pedidos.AddAsync(pedidoExistente);
            await context.SaveChangesAsync();

            var pedidoDto = new PedidoDto
            {
                Id = pedidoExistente.Id,
                Status = (int)StatusPedido.AguardandoPagamento,
                DataCriacao = DateTime.Now
            };

            // Act
            var resultado = await repository.AtualizarStatusPedidoAsync(pedidoDto);
            var pedidoAtualizado = await context.Pedidos.FindAsync(pedidoExistente.Id);

            // Assert
            resultado.Should().BeTrue();
            pedidoAtualizado.Should().NotBeNull();
            pedidoAtualizado.Status.Should().Be(StatusPedido.AguardandoPagamento);
        }

        [Fact]
        public async Task AtualizarStatusPedidoAsync_DeveRetornarFalsoSePedidoNaoExistir()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var pedidoDto = new PedidoDto 
            { 
                Id = Guid.NewGuid(), 
                Status = (int)StatusPedido.EmPreparacao,
                DataCriacao = DateTime.Now
            };

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

            var cliente = CriarClienteValido();
            await context.Clientes.AddAsync(cliente);
            await context.SaveChangesAsync();

            var pedidoDto = new PedidoDto
            {
                Id = Guid.NewGuid(),
                Cliente = new ClienteDto { Id = cliente.Id, Nome = cliente.Nome },
                Itens = [],
                DataCriacao = DateTime.Now
            };

            // Act
            var resultado = await repository.InserirPedidoAsync(pedidoDto);           

            // Assert
            resultado.Should().BeTrue();
            (await context.Pedidos.CountAsync()).Should().Be(1);
        }

        [Fact]
        public async Task InserirPedidoAsync_DeveManterEstadoDoClienteUnchanged()
        {
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);

            var cliente = CriarClienteValido();
            await context.Clientes.AddAsync(cliente);
            await context.SaveChangesAsync();

            // Criar um novo contexto para evitar rastreamento
            await using var newContext = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(newContext);

            var pedidoDto = new PedidoDto
            {
                Id = Guid.NewGuid(),
                Cliente = new ClienteDto { Id = cliente.Id, Nome = cliente.Nome },
                Itens = [],
                DataCriacao = DateTime.Now
            };

            // Act
            await repository.InserirPedidoAsync(pedidoDto);

            // Assert
            var entry = context.Entry(cliente);
            entry.State.Should().Be(EntityState.Unchanged);
        }

        #endregion

        #region PesquisarPedidosPorClienteIdAsync

        [Fact]
        public async Task PesquisarPedidosPorClienteIdAsync_DeveRetornarPedidosDoCliente()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var cliente = CriarClienteValido();
            await context.Clientes.AddAsync(cliente);
            await context.SaveChangesAsync();

            var pedido = CriarPedidoValido([], cliente);
            await context.Pedidos.AddAsync(pedido);
            await context.SaveChangesAsync();

            // Act
            var pedidos = await repository.PesquisarPedidosPorClienteIdAsync(cliente.Id);

            // Assert
            pedidos.Should().NotBeEmpty();
            pedidos.Should().HaveCount(1);
            pedidos.First().Id.Should().Be(pedido.Id);
        }

        [Fact]
        public async Task PesquisarPedidosPorClienteIdAsync_DeveRetornarListaVazia_SeNaoHouverPedidos()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var clienteId = Guid.NewGuid();

            // Act
            var pedidos = await repository.PesquisarPedidosPorClienteIdAsync(clienteId);

            // Assert
            pedidos.Should().BeEmpty();
        }

        #endregion

        #region PesquisarPedidosPorStatusAsync

        [Fact]
        public async Task PesquisarPedidosPorStatusAsync_DeveRetornarPedidosComStatusInformado()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var cliente = CriarClienteValido();
            await context.Clientes.AddAsync(cliente);

            var pedido1 = new Pedido { Id = Guid.NewGuid(), Cliente = cliente, Status = StatusPedido.Iniciado, DataCriacao = DateTime.Now };
            var pedido2 = new Pedido { Id = Guid.NewGuid(), Cliente = cliente, Status = StatusPedido.AguardandoPagamento, DataCriacao = DateTime.Now };
            await context.Pedidos.AddRangeAsync(pedido1, pedido2);
            await context.SaveChangesAsync();

            var statusPesquisado = new[] { (int)StatusPedido.AguardandoPagamento };

            // Act
            var resultado = await repository.PesquisarPedidosPorStatusAsync(statusPesquisado);

            // Assert
            resultado.Should().NotBeEmpty();
            resultado.Should().HaveCount(1);
            resultado.First().Id.Should().Be(pedido2.Id);
        }

        [Fact]
        public async Task PesquisarPedidosPorStatusAsync_DeveRetornarListaVazia_SeNenhumPedidoTiverStatusInformado()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var cliente = CriarClienteValido();
            await context.Clientes.AddAsync(cliente);

            var pedido = CriarPedidoValido([], cliente);
            await context.Pedidos.AddAsync(pedido);
            await context.SaveChangesAsync();

            var statusPesquisado = new[] { (int)StatusPedido.AguardandoPagamento };

            // Act
            var resultado = await repository.PesquisarPedidosPorStatusAsync(statusPesquisado);

            // Assert
            resultado.Should().BeEmpty();
        }

        #endregion

        #region PesquisarPorIdentificacaoAsync

        [Fact]
        public async Task PesquisarPorIdentificacaoAsync_DeveRetornarPedido_SeEncontrado()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var cliente = CriarClienteValido();
            await context.Clientes.AddAsync(cliente);

            var pedido = CriarPedidoValido([], cliente);
            await context.Pedidos.AddAsync(pedido);
            await context.SaveChangesAsync();

            // Act
            var resultado = await repository.PesquisarPorIdentificacaoAsync(pedido.Id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(pedido.Id);
        }

        [Fact]
        public async Task PesquisarPorIdentificacaoAsync_DeveRetornarNull_SePedidoNaoEncontrado()
        {
            // Arrange
            var options = CriarOpcoesEmMemoria();
            await using var context = new RepositoryDbContext(options);
            var repository = new PedidoDataSource(context);

            var identificacaoInexistente = Guid.NewGuid();

            // Act
            var resultado = await repository.PesquisarPorIdentificacaoAsync(identificacaoInexistente);

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

        private static Pedido CriarPedidoValido(List<PedidoItem> itens, Cliente cliente)
        {
            return new Pedido 
            { 
                Id = Guid.NewGuid(), 
                Status = StatusPedido.Iniciado,
                DataCriacao = DateTime.Now,
                PagamentoId = Guid.NewGuid(),
                Cliente = cliente,
                Itens = itens
            };
        }

        private static PedidoItem CriarPedidoItemValido()
        {
            return new PedidoItem 
            { 
                Id = Guid.NewGuid(), 
                Quantidade = 1, 
                ValorTotal = 10.00m, 
                Observacao = "" 
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
