namespace SnackTech.Orders.Common.CustomExceptions
{
    public class ClienteRepositoryException : Exception
    {
        public ClienteRepositoryException(string message) : base(message)
        { }
    }
}