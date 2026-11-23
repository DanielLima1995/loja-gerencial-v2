# API da Loja

API REST da aplicação, construída com ASP.NET Core e .NET 9. A persistência usa Entity Framework Core com MySQL; a fila de status de vendas usa RabbitMQ.

## Executar com Docker

Consulte o [README da raiz](../README.md) para subir o ambiente completo. A partir da raiz do repositório, use o Compose principal:

```powershell
docker compose -f .\docker-compose.yml up --build
```

A API fica em `http://localhost:5161`; o Swagger fica em `http://localhost:5161/swagger`. O Compose conecta a API ao MySQL pelo serviço `mysql` e ao RabbitMQ existente no host pela porta `5672`.

O banco Docker começa vazio. Para criar o esquema, depois de subir os serviços pela primeira vez, execute na raiz do repositório em PowerShell (requer a ferramenta `dotnet-ef`):

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=127.0.0.1;Port=3306;Database=loja;Uid=loja;Pwd=loja_dev_password;"
dotnet ef database update --project api\Loja.Infrastructure\Loja.Infrastructure.csproj --startup-project api\Loja.API\Loja.API.csproj --connection $env:ConnectionStrings__DefaultConnection
Remove-Item Env:ConnectionStrings__DefaultConnection
```

## Depurar no Visual Studio

Abra `api/Loja.slnx` ou a solução da raiz e defina `Loja.API` como projeto de inicialização. Use o perfil `https` para `https://localhost:7100` (também aceita HTTP em `http://localhost:5200`) ou `http` para `http://localhost:5200`.

Os perfis locais usam `appsettings.Development.json` e sobrescrevem as configurações de RabbitMQ, conexão MySQL e JWT pelo `launchSettings.json`. Com os serviços Docker ativos, a connection string local acessa o banco em `localhost:3306` usando o usuário `loja`. Se mudar as credenciais do MySQL no Compose, atualize também a connection string nos perfis da IDE.

A porta HTTP da API no Docker é `5161`, separada das portas de depuração `5200`/`7100`.

## Migrações

As migrações ficam em `Loja.Infrastructure/Migrations`. A API não aplica migrações automaticamente ao iniciar. Para adicionar uma migração, execute na raiz do repositório:

```powershell
dotnet ef migrations add NomeDaMigracao --project api\Loja.Infrastructure\Loja.Infrastructure.csproj --startup-project api\Loja.API\Loja.API.csproj
```

Para atualizar o banco local, use o comando `dotnet ef database update` acima. Ajuste a connection string conforme o banco de destino; não versione credenciais de produção.

## Configuração

Os arquivos locais `Loja.API/appsettings.json` e `Loja.API/appsettings.Development.json` são ignorados pelo Git. O template de desenvolvimento versionado é `Loja.API/appsettings.Development.example.json`. Se o arquivo local ainda não existir, crie-o a partir do template e substitua os valores de exemplo:

```powershell
Copy-Item api\Loja.API\appsettings.Development.example.json api\Loja.API\appsettings.Development.json
```

Variáveis de ambiente usam a convenção do .NET, por exemplo `JwtSettings__SecretKey`, `ConnectionStrings__DefaultConnection` e `RabbitMQ__Host`. Os perfis de depuração do Visual Studio também definem esses valores pelo `launchSettings.json`.

A chave JWT precisa ter pelo menos 128 bits para HS256. As chaves e credenciais dos exemplos são apenas para desenvolvimento; use valores secretos próprios em ambientes compartilhados ou produção.
