using TechTalk.SpecFlow;

namespace SnackTech.Orders.Driver.API.Functional.Tests.ControllersFunctionalTests.Steps;

[Binding]
public class PedidosControllerSteps
{
    private readonly HttpClient _client;
    private HttpResponseMessage _response;

    public PedidosControllerSteps(HttpClient client)
    {
        _client = client;
    }

    [Given(@"que eu tenho um CPF válido")]
    public void GivenQueEuTenhoUmCPFValido()
    {
        // Setup CPF válido
    }

    [When(@"eu envio uma solicitação para iniciar um pedido")]
    public async Task WhenEuEnvioUmaSolicitacaoParaIniciarUmPedido()
    {
        var content = new StringContent("{\"Cpf\": \"12345678900\"}", System.Text.Encoding.UTF8, "application/json");
        _response = await _client.PostAsync("/api/pedidos", content);
    }

    [Then(@"eu devo receber um identificador de pedido")]
    public async Task ThenEuDevoReceberUmIdentificadorDePedido()
    {
        _response.EnsureSuccessStatusCode();
        var responseContent = await _response.Content.ReadAsStringAsync();
        Assert.NotNull(responseContent);
    }

    [Given(@"que eu tenho um pedido existente que não está aguardando pagamento")]
    public void GivenQueEuTenhoUmPedidoExistenteQueNaoEstaAguardandoPagamento()
    {
        // Setup pedido existente
    }

    [When(@"eu envio uma solicitação para atualizar o pedido")]
    public async Task WhenEuEnvioUmaSolicitacaoParaAtualizarOPedido()
    {
        var content = new StringContent("{\"IdentificacaoPedido\": \"123\", \"PedidoItens\": []}", System.Text.Encoding.UTF8, "application/json");
        _response = await _client.PutAsync("/api/pedidos", content);
    }

    [Then(@"eu devo receber uma confirmação de sucesso")]
    public void ThenEuDevoReceberUmaConfirmacaoDeSucesso()
    {
        _response.EnsureSuccessStatusCode();
    }

    [Given(@"que eu tenho um pedido existente")]
    public void GivenQueEuTenhoUmPedidoExistente()
    {
        // Setup pedido existente
    }

    [When(@"eu envio uma solicitação para finalizar o pedido para pagamento")]
    public async Task WhenEuEnvioUmaSolicitacaoParaFinalizarOPedidoParaPagamento()
    {
        _response = await _client.PatchAsync("/api/pedidos/finalizar-para-pagamento/123", null);
    }

    [When(@"eu envio uma solicitação para listar pedidos aguardando pagamento")]
    public async Task WhenEuEnvioUmaSolicitacaoParaListarPedidosAguardandoPagamento()
    {
        _response = await _client.GetAsync("/api/pedidos/aguardando-pagamento");
    }

    [Then(@"eu devo receber uma lista de pedidos")]
    public async Task ThenEuDevoReceberUmaListaDePedidos()
    {
        _response.EnsureSuccessStatusCode();
        var responseContent = await _response.Content.ReadAsStringAsync();
        Assert.NotNull(responseContent);
    }

    [Given(@"que eu tenho um identificador de pedido válido")]
    public void GivenQueEuTenhoUmIdentificadorDePedidoValido()
    {
        // Setup identificador de pedido válido
    }

    [When(@"eu envio uma solicitação para buscar o pedido por identificação")]
    public async Task WhenEuEnvioUmaSolicitacaoParaBuscarOPedidoPorIdentificacao()
    {
        _response = await _client.GetAsync("/api/pedidos/123");
    }

    [Then(@"eu devo receber os detalhes do pedido")]
    public async Task ThenEuDevoReceberOsDetalhesDoPedido()
    {
        _response.EnsureSuccessStatusCode();
        var responseContent = await _response.Content.ReadAsStringAsync();
        Assert.NotNull(responseContent);
    }

    [When(@"eu envio uma solicitação para buscar o último pedido do cliente")]
    public async Task WhenEuEnvioUmaSolicitacaoParaBuscarOUltimoPedidoDoCliente()
    {
        _response = await _client.GetAsync("/api/pedidos/ultimo-pedido-cliente/12345678900");
    }

    [When(@"eu envio uma solicitação para listar pedidos ativos")]
    public async Task WhenEuEnvioUmaSolicitacaoParaListarPedidosAtivos()
    {
        _response = await _client.GetAsync("/api/pedidos/ativos");
    }

    [When(@"eu envio uma solicitação para iniciar a preparação do pedido")]
    public async Task WhenEuEnvioUmaSolicitacaoParaIniciarAPreparacaoDoPedido()
    {
        _response = await _client.PatchAsync("/api/pedidos/iniciar-preparacao/123", null);
    }

    [When(@"eu envio uma solicitação para concluir a preparação do pedido")]
    public async Task WhenEuEnvioUmaSolicitacaoParaConcluirAPreparacaoDoPedido()
    {
        _response = await _client.PatchAsync("/api/pedidos/concluir-preparacao/123", null);
    }

    [When(@"eu envio uma solicitação para finalizar o pedido")]
    public async Task WhenEuEnvioUmaSolicitacaoParaFinalizarOPedido()
    {
        _response = await _client.PatchAsync("/api/pedidos/finalizar/123", null);
    }
}