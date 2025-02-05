# language: pt

Funcionalidade: PedidosController

  Cenario: Iniciar um novo pedido
    Given que eu tenho um CPF válido
    When eu envio uma solicitação para iniciar um pedido
    Then eu devo receber um identificador de pedido

  Cenario: Atualizar um pedido
    Given que eu tenho um pedido existente que não está aguardando pagamento
    When eu envio uma solicitação para atualizar o pedido
    Then eu devo receber uma confirmação de sucesso

  Cenario: Finalizar pedido para pagamento
    Given que eu tenho um pedido existente
    When eu envio uma solicitação para finalizar o pedido para pagamento
    Then eu devo receber uma confirmação de sucesso

  Cenario: Listar pedidos aguardando pagamento
    When eu envio uma solicitação para listar pedidos aguardando pagamento
    Then eu devo receber uma lista de pedidos

  Cenario: Buscar pedido por identificação
    Given que eu tenho um identificador de pedido válido
    When eu envio uma solicitação para buscar o pedido por identificação
    Then eu devo receber os detalhes do pedido

  Cenario: Buscar último pedido do cliente
    Given que eu tenho um CPF válido
    When eu envio uma solicitação para buscar o último pedido do cliente
    Then eu devo receber os detalhes do último pedido

  Cenario: Listar pedidos ativos
    When eu envio uma solicitação para listar pedidos ativos
    Then eu devo receber uma lista de pedidos ativos

  Cenario: Iniciar preparação de um pedido
    Given que eu tenho um identificador de pedido válido
    When eu envio uma solicitação para iniciar a preparação do pedido
    Then eu devo receber uma confirmação de sucesso

  Cenario: Concluir preparação de um pedido
    Given que eu tenho um identificador de pedido válido
    When eu envio uma solicitação para concluir a preparação do pedido
    Then eu devo receber uma confirmação de sucesso

  Cenario: Finalizar um pedido
    Given que eu tenho um identificador de pedido válido
    When eu envio uma solicitação para finalizar o pedido
    Then eu devo receber uma confirmação de sucesso