# loja-gerencial-v2

## Executar front e API com Docker Compose

Com o RabbitMQ atual em execução e as portas `5672` e `15672` publicadas no host, execute na raiz do repositório:

```powershell
docker compose up --build
```

O front ficará disponível em `http://localhost:4200` e a API em `http://localhost:5161` (Swagger em `/swagger`). O RabbitMQ já existente continua em execução e é acessado pela API através do host na porta `5672`.

Para depurar a API pelo Visual Studio, selecione o perfil `https` do projeto `Loja.API`: ele usa `https://localhost:7100` e `http://localhost:5200`. O perfil `http` usa `http://localhost:5200`. Essas portas são independentes da porta Docker `5161`, então os dois processos podem ficar ativos juntos. O front executado localmente pode continuar apontando para a API HTTPS em `https://localhost:7100`; o front no Docker usa `/api` e encaminha as chamadas para a API do Compose.

O Compose executa a API em `Development`, carregando `appsettings.json` e, por cima dele, `appsettings.Development.json`. A chave `TEST_SECRET_KEY` desse último é apenas um placeholder curto; o Compose e os perfis de depuração do Visual Studio sobrescrevem essa chave com chaves de desenvolvimento de tamanho válido. Para Docker, defina `JWT_SECRET_KEY` (pelo menos 32 caracteres ASCII) se quiser usar uma chave própria. Não use as chaves de desenvolvimento em produção.

A API usa o MySQL instalado no host por padrão (`host.docker.internal:3306`, banco `loja`, usuário `root` sem senha). Para configurar outras credenciais, defina as variáveis antes de iniciar o Compose. Exemplo no PowerShell:

```powershell
$env:JWT_SECRET_KEY = "substitua-por-uma-chave-aleatoria-com-32-caracteres-ou-mais"
$env:DB_CONNECTION_STRING = "Server=host.docker.internal;Port=3306;Database=loja;Uid=root;Pwd=sua-senha;"
docker compose up --build
```

Depois de alterar a chave, recrie a API para aplicar a configuração: `docker compose up -d --force-recreate api`.
