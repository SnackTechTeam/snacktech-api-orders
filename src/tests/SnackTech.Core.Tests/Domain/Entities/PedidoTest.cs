using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Domain.Types;

namespace SnackTech.Orders.Core.Tests.Domain.Entities
{
    public class PedidoTest
    {
        [Fact]
        public void Pedido_Construtor_NaoAceitaIdNulo()
        {
            Assert.Throws<ArgumentException>(() => new Pedido(null, new DataPedidoValida(DateTime.Now), new StatusPedidoValido(1), new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191")));
        }

        [Fact]
        public void Pedido_Construtor_NaoAceitaDataCriacaoInvalida()
        {
            Assert.Throws<ArgumentException>(() => new Pedido(Guid.NewGuid(), DateTime.MinValue, new StatusPedidoValido(1), new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191")));
            Assert.Throws<ArgumentException>(() => new Pedido(Guid.NewGuid(), DateTime.Now.AddDays(1), new StatusPedidoValido(1), new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191")));
        }

        [Fact]
        public void Pedido_Construtor_NaoAceitaStatusInvalido()
        {
            Assert.Throws<ArgumentException>(() => new Pedido(Guid.NewGuid(), new DataPedidoValida(DateTime.Now), 99, new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191")));
            Assert.Throws<ArgumentException>(() => new Pedido(Guid.NewGuid(), new DataPedidoValida(DateTime.Now), 0, new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191")));
        }

        [Fact]
        public void Pedido_Construtor_NaoAceitaClienteNulo()
        {
            Assert.Throws<ArgumentException>(() => new Pedido(Guid.NewGuid(), new DataPedidoValida(DateTime.Now), new StatusPedidoValido(1), null));
        }

        [Fact]
        public void Pedido_Construtor_CriaObjetoComValoresCorretos()
        {
            var id = Guid.NewGuid();
            var dataCriacao = new DataPedidoValida(DateTime.Now);
            var status = new StatusPedidoValido(1);
            var cliente = new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191");

            var pedido = new Pedido(id, dataCriacao, status, cliente);

            Assert.Equal(id, pedido.Id.Valor);
            Assert.Equal(dataCriacao, pedido.DataCriacao);
            Assert.Equal(status, pedido.Status);
            Assert.Equal(cliente, pedido.Cliente);
        }

        [Fact]
        public void Pedido_CriarPedidoComItens_Corretamente()
        {
            var id = Guid.NewGuid();
            var dataCriacao = new DataPedidoValida(DateTime.Now);
            var status = new StatusPedidoValido(1);
            var cliente = new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191");
            var itens = new List<PedidoItem> { new PedidoItem(Guid.NewGuid(), new Produto(Guid.NewGuid(), 1), 1, "observacao") };

            var pedido = new Pedido(id, dataCriacao, status, cliente, itens);

            Assert.Equal(itens, pedido.Itens);
        }

        [Fact]
        public void Pedido_FecharPedidoParaPagamento_NaoAceitaItensVazios()
        {
            var pedido = new Pedido(Guid.NewGuid(), new DataPedidoValida(DateTime.Now), new StatusPedidoValido(1), new Cliente(Guid.NewGuid(), "nome", "email@email.com", "00000000191"));

            var ex = Assert.Throws<ArgumentException>(() => pedido.FecharPedidoParaPagamento());
            Assert.Equal("O pedido deve ter pelo menos um item para ser finalizado para pagamento.", ex.Message);
        }

        [Fact]
        public void Pedido_FecharPedidoParaPagamento_NaoAceitaValorTotalInvalido()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(new Produto(Guid.NewGuid(), 0))],
                new StatusPedidoValido(1));

            //Act and Assert
            var ex = Assert.Throws<ArgumentException>(() => pedido.FecharPedidoParaPagamento());
            Assert.Equal("O cálculo do Valor total do pedido está resultando em um valor menor ou igual a zero.", ex.Message);
        }

        [Fact]
        public void Pedido_FecharPedidoParaPagamento_StatusDiferenteDeIniciado()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(2));

            //Act and Assert
            var ex = Assert.Throws<ArgumentException>(() => pedido.FecharPedidoParaPagamento());
            Assert.Equal("Pedido está com status diferente de Iniciado, não será possível movê-lo para aguardar pagamento", ex.Message);
        }

        [Fact]
        public void Pedido_FecharPedidoParaPagamento_AlteraStatusCorretamente()
        {
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(1));

            pedido.FecharPedidoParaPagamento();

            Assert.Equal(StatusPedidoValido.AguardandoPagamento, pedido.Status);
        }

        [Fact]
        public void Pedido_IniciarPreparacao_StatusDiferenteDeRecebido()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(1));

            //Act and Assert
            var ex = Assert.Throws<ArgumentException>(() => pedido.IniciarPreparacao());
            Assert.Equal("O pedido deve estar com o status Recebido para iniciar o preparo.", ex.Message);
        }

        [Fact]
        public void Pedido_IniciarPreparacao_AlteraStatusCorretamente()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(3));

            //Act
            pedido.IniciarPreparacao();

            //Assert
            Assert.Equal(StatusPedidoValido.EmPreparacao, pedido.Status);
        }

        [Fact]
        public void Pedido_AtualizarPedidoAposPagamento_StatusDiferenteDeAguardandoPagamento()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(1));

            //Act and Assert
            var ex = Assert.Throws<ArgumentException>(() => pedido.AtualizarPedidoAposPagamento());
            Assert.Equal("O pedido deve estar com o status AguardandoPagamento.", ex.Message);
        }

        [Fact]
        public void Pedido_AtualizarPedidoAposPagamento_AlteraStatusCorretamente()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(2));

            //Act
            pedido.AtualizarPedidoAposPagamento();

            //Assert
            Assert.Equal(StatusPedidoValido.Recebido, pedido.Status);
        }

        [Fact]
        public void Pedido_ConcluirPreparacao_StatusDiferenteDeEmPreparacao()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(1));

            //Act and Assert
            var ex = Assert.Throws<ArgumentException>(() => pedido.ConcluirPreparacao());
            Assert.Equal("O pedido deve estar com o status EmPreparacao para concluir o preparo.", ex.Message);
        }

        [Fact]
        public void Pedido_ConcluirPreparacao_AlteraStatusCorretamente()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(4));

            //Act
            pedido.ConcluirPreparacao();

            //Assert
            Assert.Equal(StatusPedidoValido.Pronto, pedido.Status);
        }

        [Fact]
        public void Pedido_Finalizar_AlteraStatusCorretamente()
        {
            //Arrange
            var pedido = CriarPedido(
                CriarClienteValido(),
                [CriarPedidoItem(CriarProdutoValido())],
                new StatusPedidoValido(5));

            //Act
            pedido.Finalizar();

            //Assert
            Assert.Equal(StatusPedidoValido.Finalizado, pedido.Status);
        }

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
    }
}