Feature: PedidosController
  Como usuário do sistema de pedidos
  Quero gerenciar pedidos de forma eficiente
  Para garantir uma boa experiência do cliente

  Background:
    Given que eu tenho um CPF válido

  @Criacao
  Scenario: Iniciar um novo pedido
    When eu envio uma solicitação para iniciar um pedido
    Then eu devo receber um identificador de pedido

  @Atualizacao
  Scenario: Atualizar um pedido existente
    Given que eu tenho um pedido que não está aguardando pagamento
    When eu envio uma solicitação para atualizar o pedido
    Then eu devo receber uma confirmação de sucesso

  @Finalizacao
  Scenario: Finalizar um pedido para pagamento
    Given que eu tenho um pedido existente
    When eu envio uma solicitação para finalizar o pedido para pagamento
    Then eu devo receber uma confirmação de sucesso

  @Listagem
  Scenario: Listar pedidos aguardando pagamento
    When eu envio uma solicitação para listar pedidos aguardando pagamento
    Then eu devo receber uma lista de pedidos

  @Consulta
  Scenario: Buscar pedido por identificação
    Given que eu tenho um identificador de pedido válido
    When eu envio uma solicitação para buscar o pedido por identificação
    Then eu devo receber os detalhes do pedido

  @Consulta
  Scenario: Buscar último pedido do cliente
    When eu envio uma solicitação para buscar o último pedido do cliente
    Then eu devo receber os detalhes do último pedido

  @Listagem
  Scenario: Listar pedidos ativos
    When eu envio uma solicitação para listar pedidos ativos
    Then eu devo receber uma lista de pedidos ativos

  @Preparacao
  Scenario: Iniciar preparação de um pedido
    Given que eu tenho um identificador de pedido válido
    When eu envio uma solicitação para iniciar a preparação do pedido
    Then eu devo receber uma confirmação de sucesso

  @Preparacao
  Scenario: Concluir preparação de um pedido
    Given que eu tenho um identificador de pedido válido
    When eu envio uma solicitação para concluir a preparação do pedido
    Then eu devo receber uma confirmação de sucesso

  @Finalizacao
  Scenario: Finalizar um pedido
    Given que eu tenho um identificador de pedido válido
    When eu envio uma solicitação para finalizar o pedido
    Then eu devo receber uma confirmação de 