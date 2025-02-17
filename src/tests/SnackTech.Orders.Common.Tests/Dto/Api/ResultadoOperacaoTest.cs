using FluentAssertions;
using SnackTech.Orders.Common.Dto.Api;

namespace SnackTech.Orders.Common.Tests.Dto.Api
{
    public class ResultadoOperacaoTest
    {
        [Fact]
        public void ResultadoOperacao_QuandoHouveErroForFalse_DeveLancarArgumentException()
        {
            // Act
            Action act = () => new ResultadoOperacao<string>("mensagem", false);

            //Assert
            act.Should()
            .Throw<ArgumentException>()
            .WithMessage("Use ResultadoOperacao<string>(string) como construtor para resultados de sucesso.*");
        }
    }
}