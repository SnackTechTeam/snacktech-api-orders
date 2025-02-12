using SnackTech.Orders.Core.Domain.Entities;

namespace SnackTech.Orders.Core.Tests.Domain.Entities
{
    public class ProdutoTest
    {
        [Fact]
        public void Produto_Construtor_NaoAceitaIdNulo()
        {
            Assert.Throws<ArgumentException>(() => new Produto(null, 1));
        }

        [Fact]
        public void Produto_Construtor_NaoAceitaValoMenorOuIgualAZero()
        {
            Assert.Throws<ArgumentException>(() => new Produto(Guid.NewGuid(), -1));
        }

        [Fact]
        public void Produto_Construtor_CriaObjetoComValoresCorretos()
        {
            var id = Guid.NewGuid();
            var valor = 10.99m;

            var produto = new Produto(id, valor);

            Assert.Equal(id, produto.Id.Valor);
            Assert.Equal(valor, produto.Valor.Valor);
        }
    }
}