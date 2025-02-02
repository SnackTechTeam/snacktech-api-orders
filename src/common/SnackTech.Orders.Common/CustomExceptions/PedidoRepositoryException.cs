namespace SnackTech.Orders.Common.CustomExceptions
{
    public class PedidoRepositoryException : Exception
    {
        public PedidoRepositoryException(string message) : base(message)
        { }
    }
}