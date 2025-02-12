using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.CustomExceptions
{
    [ExcludeFromCodeCoverage]
    public class ClienteRepositoryException : Exception
    {
        public ClienteRepositoryException(string message) : base(message)
        { }
    }
}