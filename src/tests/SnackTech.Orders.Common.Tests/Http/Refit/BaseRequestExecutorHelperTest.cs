using FluentAssertions;
using Moq;
using Refit;
using SnackTech.Orders.Common.Http.Refit;
using System.Net;

namespace SnackTech.Orders.Common.Tests.Http.Refit
{
    public class BaseRequestExecutorHelperTest
    {
        private readonly BaseRequestExecutorHelper _service;

        public BaseRequestExecutorHelperTest()
        {
            _service = new BaseRequestExecutorHelper();
        }

        [Fact]
        public async Task Execute_ShouldReturnSuccess_WhenResponseIsSuccessfulAndHasContent()
        {
            // Arrange
            var expectedContent = "Test Data";
            var apiResponse = await CreateApiResponse(HttpStatusCode.OK, expectedContent, false);

            var mockFunc = new Mock<Func<Task<ApiResponse<string>>>>();
            mockFunc
                .Setup(m => m.Invoke())
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _service.Execute(mockFunc.Object);

            // Assert
            result.Sucesso.Should().BeTrue();
            result.Dados.Should().Be(expectedContent);            
        }

        [Fact]
        public async Task Execute_ShouldReturnErrorMessage_WhenExceptionOccurs()
        {
            // Arrange
            var exceptionMessage = "Error occurred";

            var mockFunc = new Mock<Func<Task<ApiResponse<string>>>>();
            mockFunc
                .Setup(f => f.Invoke())
                .ThrowsAsync(new InvalidOperationException(exceptionMessage));

            // Act
            var result = await _service.Execute(mockFunc.Object);

            // Assert
            result.Sucesso.Should().BeFalse();
            result.Dados.Should().BeNull();
            result.Excecao.Should().NotBeNull();
            result.Excecao.Message.Should().Contain(exceptionMessage);
        }

        [Fact]
        public async Task Execute_ShouldReturnErrorResponse_WhenApiResponseIsUnsuccessful()
        {
            // Arrange
            var errorMessage = "Error occurred";
            var apiResponse = await CreateApiResponse(
                HttpStatusCode.BadRequest,
                errorMessage,
                true
            );

            var mockFunc = new Mock<Func<Task<ApiResponse<string>>>>();
            mockFunc
                .Setup(f => f.Invoke())
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _service.Execute(mockFunc.Object);

            // Assert
            result.Sucesso.Should().BeFalse();
            result.Mensagem.Should().Be(errorMessage);
        }

        private static async Task<ApiResponse<string>> CreateApiResponse(HttpStatusCode statusCode, string? content, bool withException)
        {
            var responseMessage = new HttpResponseMessage(statusCode);

            if (content != null)
                responseMessage.Content = new StringContent(content.ToString());

            if (withException)
            {
                ApiException ex = await ApiException.Create(
                   new HttpRequestMessage(),
                   HttpMethod.Get,
                   responseMessage,
                   new RefitSettings(),
                   new Exception());

                return new ApiResponse<string>(responseMessage, content, null, ex);
            }
            else
                return new ApiResponse<string>(responseMessage, content, null);
        }
    }
}
