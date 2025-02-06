using System.Text;

namespace SnackTech.Orders.Core.Tests
{
    public class TestSetup
    {
        static TestSetup()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;
        }
    }
}
