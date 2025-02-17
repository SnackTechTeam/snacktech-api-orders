# snacktech-api-orders

## 🛠️ Sobre o Projeto

Serviço API que lida com as features que envolvem cadastro de clientes e criação de pedidos bem como a evolução de todos os seus estágios, desde o pedido iniciado até o pedido finalizado.

## 💻 Tecnologias utilizadas

- **C#**: Linguagem de programação usada no desenvolvimento do projeto
- **.NET 8**: Framework como base em que a API é executada
- **Sql Server**: Base de dados para armazenar os dados trabalhados pela API
- **Swagger**: Facilita a documentação da API
- **Docker**: Permite criar uma imagem do serviço e rodá-la em forma de contâiner

## Como Utilizar

## 🛡️ Pré-requisitos

Antes de rodar o projeto SnackTech, certifique-se de que você possui os seguintes pré-requisitos:

- **.NET SDK**: O projeto foi desenvolvido com o .NET SDK 8. Instale a versão necessária para garantir a compatibilidade com o código.
- **Docker**: O projeto utiliza Docker para contêinerizar a aplicação e o banco de dados. Instale o Docker Desktop para Windows ou Mac, ou configure o Docker Engine para Linux. O Docker Compose também é necessário para orquestrar os containers.
- **Sql Server (Opcional)**: O projeto tem um arquivo de docker-compose que configura e gerencia uma instância do Sql Server dentro de um container Docker. Sendo assim, a instalação ou uso de uma solução em nuvem é opcional.

## 💡 Instalação e Execução Local

Esta API depende das APIs de Produtos, Pagamentos e do worker de Pagamentos. Veja mais sobre cada aplicação em seus respectivos repositórios:
- [snacktech-api-products](https://github.com/SnackTechTeam/snacktech-api-products)
- [snacktech-api-payment](https://github.com/SnackTechTeam/snacktech-api-payment)
- [snacktech-worker-payment](https://github.com/SnackTechTeam/snacktech-worker-payment)

Com os serviços redando, seja localmente em containers ou na nuvem, é preciso configurar no appsettings.json a url base de cada serviço da seguinte forma:

```json
"ProdutoApiSettings": {
  "EnableIntegration": true,
  "UrlBase": "https://localhost:44360/api"
},
"PagamentoApiSettings": {
  "EnableIntegration": true,
  "UrlBase": "https://localhost:44361/api"
}
```

Agora o próximo passo é executar o docker compose. Em seu console navegue até o diretório /src e execute o seguinte comando:
```
docker compose up -d
```
Dessa forma seus containers iniciarão em background.

## Equipe

* Adriano de Melo Costa. Email: adriano.dmcosta@gmail.com
* Rafael Duarte Gervásio da Silva. Email: rafael.dgs.1993@gmail.com
* Guilherme Felipe de Souza. Email: gui240799@outlook.com
* Dayvid Ribeiro Correia. Email: dayvidrc@gmail.com
