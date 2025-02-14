# Use uma imagem base do .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Define o diretório de trabalho no contêiner
WORKDIR /src

# Copie os arquivos .csproj para o contêiner
COPY common/SnackTech.Orders.Common/SnackTech.Orders.Common.csproj common/SnackTech.Orders.Common/
COPY core/SnackTech.Orders.Core/SnackTech.Orders.Core.csproj core/SnackTech.Orders.Core/
COPY infra.persistence/SnackTech.Orders.Driver.DataBase/SnackTech.Orders.Driver.DataBase.csproj infra.persistence/SnackTech.Orders.Driver.DataBase/
COPY infra.persistence/SnackTech.Orders.Driver.Payments/SnackTech.Orders.Driver.Payments.csproj infra.persistence/SnackTech.Orders.Driver.Payments/
COPY infra.persistence/SnackTech.Orders.Driver.Products/SnackTech.Orders.Driver.Products.csproj infra.persistence/SnackTech.Orders.Driver.Products/
COPY infra.web-api/SnackTech.Orders.Driver.API/SnackTech.Orders.Driver.API.csproj infra.web-api/SnackTech.Orders.Driver.API/

# Restaure as dependências
RUN dotnet restore infra.web-api/SnackTech.Orders.Driver.API/SnackTech.Orders.Driver.API.csproj

# Copie todos os arquivos restantes
COPY . ./

# Compile o projeto
RUN dotnet build infra.web-api/SnackTech.Orders.Driver.API/SnackTech.Orders.Driver.API.csproj -c Release -o /app/build

# Use a imagem de runtime do .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Define o diretório de trabalho no contêiner
WORKDIR /src

# Copie a build da etapa anterior
COPY --from=build-env /app/build .

# Exponha a porta da API
EXPOSE 8080
EXPOSE 8081

# Defina o comando para rodar a aplicação
ENTRYPOINT ["dotnet", "SnackTech.Orders.Driver.API.dll"]
