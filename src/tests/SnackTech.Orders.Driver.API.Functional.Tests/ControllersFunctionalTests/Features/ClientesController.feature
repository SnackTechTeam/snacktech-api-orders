# language: pt

Funcionalidade: ClientesController

  Cenario: Cadastrar um novo cliente
    Given que eu tenho os dados de um novo cliente
    When eu envio uma solicitação POST para "/api/clientes"
    Then o cliente deve ser criado com sucesso
    And eu devo receber um status code 200

  Cenario: Obter cliente por CPF
    Given que eu tenho o CPF de um cliente existente
    When eu envio uma solicitação GET para "/api/clientes/{cpf}"
    Then eu devo receber os dados do cliente
    And eu devo receber um status code 200

  Cenario: Obter cliente padrão
    Given que eu quero obter o cliente padrão
    When eu envio uma solicitação GET para "/api/clientes/cliente-padrao"
    Then eu devo receber os dados do cliente padrão
    And eu devo receber um status code 200
