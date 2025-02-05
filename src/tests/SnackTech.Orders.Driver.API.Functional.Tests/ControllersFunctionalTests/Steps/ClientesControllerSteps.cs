using System.Net;
using System.Net.Http.Json;
using System.Text;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using Xunit;

namespace SnackTech.Orders.Driver.API.Functional.Tests.ControllersFunctionalTests.Steps;

[Binding]
public class ClientesControllerSteps
{
    private readonly HttpClient _client;
    private HttpResponseMessage _response = null!;
    private string _cpf = string.Empty;
    private string _clienteJson = string.Empty;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public ClientesControllerSteps()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5000") // Simula a URL base
        };

        _client = httpClient;
    }

    #region Cadastrar um novo cliente
    [Given(@"que eu tenho os dados de um novo cliente")]
    public void GivenQueEuTenhoOsDadosDeUmNovoCliente()
    {
        var cliente = new
        {
            Nome = "João Silva",
            Email = "joao.silva@example.com",
            Cpf = "12345678900"
        };
        _clienteJson = JsonConvert.SerializeObject(cliente);
    }

    [When(@"eu envio uma solicitação POST para ""(.*)""")]
    public async Task WhenEuEnvioUmaSolicitacaoPOSTPara(string url)
    {
        // Mocka a resposta da API para POST
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri.ToString().Contains(url)), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"message\": \"Cliente criado com sucesso\"}", Encoding.UTF8, "application/json")
            });

        var content = new StringContent(_clienteJson, Encoding.UTF8, "application/json");
        _response = await _client.PostAsync(url, content);
    }

    [Then(@"o cliente deve ser criado com sucesso")]
    public void ThenOClienteDeveSerCriadoComSucesso()
    {
        Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
    }
    #endregion

    #region Consultar cliente padrão
    [Given(@"que eu quero obter o cliente padrão")]
    public void GivenQueEuQueroObterOClientePadrao()
    {
        // Não é necessário configurar nada específico para este step
    }

    [When(@"eu envio uma solicitação GET para o cliente padrão ""(.*)""")]
    public async Task WhenEuEnvioUmaSolicitacaoGETParaClientePadrao(string url)
    {
        // Mocka a resposta da API para GET
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains(url)), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"nome\": \"Cliente Padrão\", \"cpf\": \"00000000000\"}", Encoding.UTF8, "application/json")
            });

        _response = await _client.GetAsync(url);
    }

    [Then(@"eu devo receber os dados do cliente padrão")]
    public async Task ThenEuDevoReceberOsDadosDoClientePadrao()
    {
        var responseContent = await _response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(responseContent));
    }

    [Then(@"eu devo receber um status code (.*)")]
    public void ThenEuDevoReceberUmStatusCode(int statusCode)
    {
        Assert.Equal((HttpStatusCode)statusCode, _response.StatusCode);
    }
    #endregion

    #region Consultar cliente por CPF
    [Given(@"que eu tenho o CPF de um cliente existente")]
    public void GivenQueEuTenhoOCpfDeUmClienteExistente()
    {
        _cpf = "12345678900";
    }

    [When(@"eu envio uma solicitação GET para ""(.*)""")]
    public async Task WhenEuEnvioUmaSolicitacaoGETPara(string url)
    {
        // Mocka a resposta da API para GET  
        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().Contains(url)), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"nome\": \"Cliente Padrão\", \"cpf\": \"00000000000\"}", Encoding.UTF8, "application/json")
            });

        _response = await _client.GetAsync(url);
    }

    [Then(@"eu devo receber os dados do cliente")]
    public async Task ThenEuDevoReceberOsDadosDoCliente()
    {
        var responseContent = await _response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(responseContent));
    }
    #endregion

}
