using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.CustomExceptions
{
    [ExcludeFromCodeCoverage]
    public class PedidoRepositoryException : Exception
    {
        public PedidoRepositoryException(string message) : base(message)
        { }
    }
}