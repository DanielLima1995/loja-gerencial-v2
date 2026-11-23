# loja-gerencial-v2

Sistema de loja virtual com front-end Angular e API ASP.NET Core.

- [Documentação da API](api/README.md)
- [Documentação do front-end](front/README.md)

## Executar front e API com Docker Compose

Com o RabbitMQ atual em execução e as portas `5672` e `15672` publicadas no host, pare o MySQL do XAMPP para liberar a porta `3306`. Na raiz do repositório, execute explicitamente o Compose principal (há também um Compose separado em `api` para o RabbitMQ):

```powershell
docker compose -f .\docker-compose.yml up --build
```

O Compose usa os estágios de desenvolvimento da API e do front. Os diretórios `api` e `front` são montados nos containers, então alterações salvas no código são recompiladas sem reconstruir as imagens:

- **Front:** executa `ng serve` na porta `4200`, com recarga automática e polling a cada segundo. As chamadas `/api` passam pelo proxy Angular para a API. `node_modules` e o cache do npm ficam em volumes Docker, sem sobrepor os arquivos do host.
- **API:** executa `dotnet watch run` com polling e sem carregar o `launchSettings.json` da IDE. Os diretórios `bin` e `obj` de cada projeto .NET ficam em volumes Docker próprios para não misturar artefatos Linux e Windows.

Se alterar `front/package-lock.json`, reinicie o front para sincronizar as dependências: `docker compose -f .\docker-compose.yml restart front`. O script de inicialização executa `npm ci` quando detecta uma mudança no lockfile. Mudanças nos Dockerfiles exigem `docker compose -f .\docker-compose.yml up --build`.

Para acompanhar a inicialização e os recarregamentos:

```powershell
docker compose -f .\docker-compose.yml logs -f api front
```

O front ficará disponível em `http://localhost:4200`, a API em `http://localhost:5161` (Swagger em `/swagger`) e o MySQL do container em `localhost:3306`. O RabbitMQ já existente continua em execução e é acessado pela API através do host na porta `5672`.

O RabbitMQ não faz parte do Compose principal. Se ele ainda não estiver em execução, inicie o serviço definido no Compose separado:

```powershell
docker compose -f .\api\docker-compose.yml up -d
```

O MySQL usa a imagem `mysql:8.4`; o banco `loja` e o usuário da aplicação `loja` são criados na primeira inicialização. Os dados ficam no volume Docker `mysql_data`, preservados mesmo que o container seja recriado. As credenciais locais padrão são usuário `loja`, senha `loja_dev_password` (root: `loja_dev_root_password`); defina `DB_PASSWORD` e `MYSQL_ROOT_PASSWORD` antes de subir se quiser alterá-las. Essas senhas padrão são apenas para desenvolvimento.

O banco no container começa vazio: os dados do XAMPP não são copiados automaticamente e a API não aplica as migrações ao iniciar. Depois de subir os containers pela primeira vez, aplique as migrações a partir da raiz no PowerShell (requer a ferramenta `dotnet-ef`):

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=127.0.0.1;Port=3306;Database=loja;Uid=loja;Pwd=loja_dev_password;"
dotnet ef database update --project api\Loja.Infrastructure\Loja.Infrastructure.csproj --startup-project api\Loja.API\Loja.API.csproj --connection $env:ConnectionStrings__DefaultConnection
Remove-Item Env:ConnectionStrings__DefaultConnection
```

Para manter os dados existentes, exporte o banco do XAMPP e importe o dump no MySQL do container. Depois de confirmar que não precisa mais do banco do XAMPP, pode iniciá-lo novamente apenas se alterar a porta publicada do container, pois os dois não podem usar `3306` simultaneamente.

Para depurar a API pelo Visual Studio, selecione o perfil `https` do projeto `Loja.API`: ele usa `https://localhost:7100` e `http://localhost:5200`. O perfil `http` usa `http://localhost:5200`. Esses endereços são independentes da porta Docker `5161`, então os dois processos podem ficar ativos juntos. Os perfis da IDE conectam ao MySQL do container via `localhost:3306`, com usuário `loja` e senha padrão `loja_dev_password`. Se alterar `DB_PASSWORD`, atualize também a connection string nos perfis da IDE. O front executado localmente pode continuar apontando para a API HTTPS em `https://localhost:7100`; o front no Docker usa `/api` e encaminha as chamadas para a API do Compose.

O Compose executa a API em `Development`, carregando `appsettings.json` e, por cima dele, `appsettings.Development.json`. A chave `TEST_SECRET_KEY` desse último é apenas um placeholder curto; o Compose e os perfis de depuração do Visual Studio sobrescrevem essa chave com chaves de desenvolvimento de tamanho válido. Para Docker, defina `JWT_SECRET_KEY` (pelo menos 32 caracteres ASCII) se quiser usar uma chave própria. Não use as chaves de desenvolvimento em produção.

O serviço da API monta o arquivo local de .NET User Secrets em modo somente leitura: `%APPDATA%\Microsoft\UserSecrets\f6be3041-1bbf-49bb-870e-20b9ffb277ca\secrets.json`. Esse arquivo precisa existir antes de iniciar o Compose. Se a API usar o S3, configure `AWS:AccessKeyId` e `AWS:SecretAccessKey` com `dotnet user-secrets` no projeto `api\Loja.API\Loja.API.csproj`; mantenha essas credenciais fora do repositório.

A API no Docker conecta ao serviço `mysql` pela rede Compose. Para configurar credenciais diferentes, defina `DB_PASSWORD` e `MYSQL_ROOT_PASSWORD` antes de iniciar. Exemplo no PowerShell:

```powershell
$env:JWT_SECRET_KEY = "substitua-por-uma-chave-aleatoria-com-32-caracteres-ou-mais"
$env:DB_PASSWORD = "uma-senha-local-forte"
$env:MYSQL_ROOT_PASSWORD = "outra-senha-local-forte"
docker compose -f .\docker-compose.yml up --build
```

Depois de alterar a chave, recrie a API para aplicar a configuração: `docker compose -f .\docker-compose.yml up -d --force-recreate api`.
