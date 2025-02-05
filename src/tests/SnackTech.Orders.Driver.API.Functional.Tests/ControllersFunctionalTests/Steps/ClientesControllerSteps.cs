using System.Net;
using System.Text;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace SnackTech.Orders.Driver.API.Functional.Tests.ControllersFunctionalTests.Steps;

[Binding]
public class ClientesControllerSteps
{
    private readonly HttpClient _client;
    private HttpResponseMessage _response;
    private string _cpf;
    private string _clienteJson;

    public ClientesControllerSteps()
    {
        _client = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };
    }

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
        var content = new StringContent(_clienteJson, Encoding.UTF8, "application/json");
        _response = await _client.PostAsync(url, content);
    }

    [Then(@"o cliente deve ser criado com sucesso")]
    public void ThenOClienteDeveSerCriadoComSucesso()
    {
        Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
    }

    [Given(@"que eu tenho o CPF de um cliente existente")]
    public void GivenQueEuTenhoOCpfDeUmClienteExistente()
    {
        _cpf = "12345678900";
    }

    [When(@"eu envio uma solicitação GET para ""(.*)""")]
    public async Task WhenEuEnvioUmaSolicitacaoGETPara(string url)
    {
        _response = await _client.GetAsync(url.Replace("{cpf}", _cpf));
    }

    [Then(@"eu devo receber os dados do cliente")]
    public async Task ThenEuDevoReceberOsDadosDoCliente()
    {
        var responseContent = await _response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(responseContent));
    }

    [Given(@"que eu quero obter o cliente padrão")]
    public void GivenQueEuQueroObterOClientePadrao()
    {
        // No setup needed for this step
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
}
