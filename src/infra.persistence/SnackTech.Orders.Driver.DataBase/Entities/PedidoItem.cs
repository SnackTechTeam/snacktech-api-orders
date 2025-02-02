namespace SnackTech.Orders.Driver.DataBase.Entities
{
    public class PedidoItem
    {
        public Guid Id { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorUnitario { get; set; }
        public Guid ProdutoId { get; set; }
    }
}