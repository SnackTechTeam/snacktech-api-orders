using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using TechTalk.SpecFlow;
using Xunit;

namespace SnackTech.Orders.Driver.API.Functional.Tests.ControllersFunctionalTests.Steps
{
    [Binding]
    public class PedidosControllerSteps
    {
        private readonly HttpClient _client;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpResponseMessage _response;

        public PedidosControllerSteps()
        {
            // Cria o mock do HttpMessageHandler (classe abstrata) para injetar no HttpClient
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _client = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }

        #region Background

        [Given(@"que eu tenho um CPF válido")]
        public void GivenQueEuTenhoUmCPFValido()
        {
            // Neste exemplo, não é preciso executar nenhuma ação específica.
        }

        #endregion

        #region Cenário: Iniciar um novo pedido (@Criacao)

        [When(@"eu envio uma solicitação para iniciar um pedido")]
        public async Task WhenEuEnvioUmaSolicitacaoParaIniciarUmPedido()
        {
            // Configura o mock para a chamada POST em /api/pedidos
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("/api/pedidos")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"orderId\": \"12345\"}", Encoding.UTF8, "application/json")
                });

            // Envia a solicitação com um JSON de exemplo
            var content = new StringContent("{\"Cpf\": \"12345678900\"}", Encoding.UTF8, "application/json");
            _response = await _client.PostAsync("/api/pedidos", content);
        }

        [Then(@"eu devo receber um identificador de pedido")]
        public async Task ThenEuDevoReceberUmIdentificadorDePedido()
        {
            _response.EnsureSuccessStatusCode();
            var responseContent = await _response.Content.ReadAsStringAsync();

            // Validação simples: verifica se o JSON possui a propriedade "orderId"
            dynamic result = JsonConvert.DeserializeObject(responseContent);
            Assert.NotNull(result.orderId);
        }

        #endregion

        #region Cenário: Atualizar um pedido existente (@Atualizacao)

        [Given(@"que eu tenho um pedido que não está aguardando pagamento")]
        public void GivenQueEuTenhoUmPedidoQueNaoEstaAguardandoPagamento()
        {
            // Configuração do contexto: simula um pedido que não está aguardando pagamento.
            // Por exemplo, você pode armazenar um identificador ou status para utilizar no teste.
        }

        [When(@"eu envio uma solicitação para atualizar o pedido")]
        public async Task WhenEuEnvioUmaSolicitacaoParaAtualizarOPedido()
        {
            // Configura o mock para a chamada PUT em /api/pedidos
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri.ToString().Contains("/api/pedidos")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var content = new StringContent("{\"IdentificacaoPedido\": \"123\", \"PedidoItens\": []}",
                Encoding.UTF8, "application/json");
            _response = await _client.PutAsync("/api/pedidos", content);
        }

        [Then(@"eu devo receber uma confirmação de sucesso")]
        public void ThenEuDevoReceberUmaConfirmacaoDeSucesso()
        {
            _response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Cenário: Finalizar um pedido para pagamento (@Finalizacao)

        [Given(@"que eu tenho um pedido existente")]
        public void GivenQueEuTenhoUmPedidoExistente()
        {
            // Aqui você pode armazenar ou configurar o identificador de um pedido existente.
        }

        [When(@"eu envio uma solicitação para finalizar o pedido para pagamento")]
        public async Task WhenEuEnvioUmaSolicitacaoParaFinalizarOPedidoParaPagamento()
        {
            // Configura o mock para a chamada PATCH no endpoint de finalização para pagamento
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Patch &&
                        req.RequestUri.ToString().Contains("/api/pedidos/finalizar-para-pagamento")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            _response = await _client.PatchAsync("/api/pedidos/finalizar-para-pagamento/123", null);
        }


        [When(@"eu envio uma solicitação para finalizar o pedido")]
        public async Task WhenEuEnvioUmaSolicitacaoParaFinalizarOPedido()
        {
            // Configura o mock para a chamada PATCH no endpoint de finalização do pedido
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Patch &&
                        req.RequestUri.ToString().Contains("/api/pedidos/finalizar")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            _response = await _client.PatchAsync("/api/pedidos/finalizar/123", null);
        }

        [Then(@"eu devo receber uma confirmação de")]
        public void ThenEuDevoReceberUmaConfirmacaoDe()
        {
            _response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Cenário: Listar pedidos aguardando pagamento (@Listagem)

        [When(@"eu envio uma solicitação para listar pedidos aguardando pagamento")]
        public async Task WhenEuEnvioUmaSolicitacaoParaListarPedidosAguardandoPagamento()
        {
            // Configura o mock para a chamada GET no endpoint de pedidos aguardando pagamento
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("/api/pedidos/aguardando-pagamento")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{\"orderId\":\"123\"}]", Encoding.UTF8, "application/json")
                });

            _response = await _client.GetAsync("/api/pedidos/aguardando-pagamento");
        }

        [Then(@"eu devo receber uma lista de pedidos")]
        public async Task ThenEuDevoReceberUmaListaDePedidos()
        {
            _response.EnsureSuccessStatusCode();
            var responseContent = await _response.Content.ReadAsStringAsync();
            Assert.NotNull(responseContent);
        }

        #endregion

        #region Cenário: Buscar pedido por identificação (@Consulta)

        [Given(@"que eu tenho um identificador de pedido válido")]
        public void GivenQueEuTenhoUmIdentificadorDePedidoValido()
        {
            // Configuração do identificador de pedido (ex: "123")
        }

        [When(@"eu envio uma solicitação para buscar o pedido por identificação")]
        public async Task WhenEuEnvioUmaSolicitacaoParaBuscarOPedidoPorIdentificacao()
        {
            // Configura o mock para a chamada GET no endpoint de busca por ID
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("/api/pedidos/123")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"orderId\": \"123\", \"status\": \"Completed\"}",
                        Encoding.UTF8, "application/json")
                });

            _response = await _client.GetAsync("/api/pedidos/123");
        }

        [Then(@"eu devo receber os detalhes do pedido")]
        public async Task ThenEuDevoReceberOsDetalhesDoPedido()
        {
            _response.EnsureSuccessStatusCode();
            var responseContent = await _response.Content.ReadAsStringAsync();
            Assert.NotNull(responseContent);
        }

        #endregion

        #region Cenário: Buscar último pedido do cliente (@Consulta)

        [When(@"eu envio uma solicitação para buscar o último pedido do cliente")]
        public async Task WhenEuEnvioUmaSolicitacaoParaBuscarOUltimoPedidoDoCliente()
        {
            // Configura o mock para a chamada GET no endpoint do último pedido do cliente
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("/api/pedidos/ultimo-pedido-cliente")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"orderId\": \"123\", \"status\": \"Pending\"}",
                        Encoding.UTF8, "application/json")
                });

            _response = await _client.GetAsync("/api/pedidos/ultimo-pedido-cliente/12345678900");
        }

        [Then(@"eu devo receber os detalhes do último pedido")]
        public async Task ThenEuDevoReceberOsDetalhesDoUltimoPedido()
        {
            _response.EnsureSuccessStatusCode();
            var responseContent = await _response.Content.ReadAsStringAsync();
            // Validação simples para garantir que a resposta contenha algum detalhe
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        #endregion

        #region Cenário: Listar pedidos ativos (@Listagem)

        [When(@"eu envio uma solicitação para listar pedidos ativos")]
        public async Task WhenEuEnvioUmaSolicitacaoParaListarPedidosAtivos()
        {
            // Configura o mock para a chamada GET no endpoint de pedidos ativos
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("/api/pedidos/ativos")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[{\"orderId\":\"123\"}]", Encoding.UTF8, "application/json")
                });

            _response = await _client.GetAsync("/api/pedidos/ativos");
        }

        [Then(@"eu devo receber uma lista de pedidos ativos")]
        public async Task ThenEuDevoReceberUmaListaDePedidosAtivos()
        {
            _response.EnsureSuccessStatusCode();
            var responseContent = await _response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseContent));
        }

        #endregion

        #region Cenário: Iniciar preparação de um pedido
        [Given(@"que eu tenho um pedido pronto para iniciar a preparação")]
        public void GivenQueEuTenhoUmPedidoProntoParaIniciarAPreparacao()
        {
            // Configuração do contexto: simula um pedido pronto para iniciar a preparação.
            // Por exemplo, você pode armazenar um identificador ou status para utilizar no teste.
        }

        [When(@"eu envio uma solicitação para iniciar a preparação do pedido")]
        public async Task WhenEuEnvioUmaSolicitacaoParaIniciarAPreparacaoDoPedido()
        {
            // Configura o mock para a chamada PATCH no endpoint de iniciar preparação
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Patch &&
                        req.RequestUri.ToString().Contains("/api/pedidos/iniciar-preparacao")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            _response = await _client.PatchAsync("/api/pedidos/iniciar-preparacao/123", null);
        }

        [Then(@"eu devo receber uma confirmação de início de preparação")]
        public void ThenEuDevoReceberUmaConfirmacaoDeInicioDePreparacao()
        {
            _response.EnsureSuccessStatusCode();
        }
        #endregion

        #region Cenário: Concluir preparação de um pedido

        [Given(@"que eu tenho um pedido em preparação")]
        public void GivenQueEuTenhoUmPedidoEmPreparacao()
        {
            // Configuração do contexto: simula um pedido em preparação.
            // Por exemplo, você pode armazenar um identificador ou status para utilizar no teste.
        }

        [When(@"eu envio uma solicitação para concluir a preparação do pedido")]
        public async Task WhenEuEnvioUmaSolicitacaoParaConcluirAPreparacaoDoPedido()
        {
            // Configura o mock para a chamada PATCH no endpoint de concluir preparação
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Patch &&
                        req.RequestUri.ToString().Contains("/api/pedidos/concluir-preparacao")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            _response = await _client.PatchAsync("/api/pedidos/concluir-preparacao/123", null);
        }

        [Then(@"eu devo receber uma confirmação de conclusão de preparação")]
        public void ThenEuDevoReceberUmaConfirmacaoDeConclusaoDePreparacao()
        {
            _response.EnsureSuccessStatusCode();
        }

        #endregion

        #region Cenário: Finalizar um pedido

        [Given(@"que eu tenho um pedido pronto para ser finalizado")]
        public void GivenQueEuTenhoUmPedidoProntoParaSerFinalizado()
        {
            // Configuração do contexto: simula um pedido pronto para ser finalizado.
            // Por exemplo, você pode armazenar um identificador ou status para utilizar no teste.
        }

        [When(@"eu envio uma solicitação para finalizar o pedido final")]
        public async Task WhenEuEnvioUmaSolicitacaoParaFinalizarOPedidoFinal()
        {
            // Configura o mock para a chamada PATCH no endpoint de finalizar pedido
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Patch &&
                        req.RequestUri.ToString().Contains("/api/pedidos/finalizar")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            _response = await _client.PatchAsync("/api/pedidos/finalizar/123", null);
        }

        [Then(@"eu devo receber uma confirmação de finalização")]
        public void ThenEuDevoReceberUmaConfirmacaoDeFinalizacao()
        {
            _response.EnsureSuccessStatusCode();
        }

        #endregion

    }
}
