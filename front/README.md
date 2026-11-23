# Front-end da Loja

Aplicação web construída com Angular 19, TypeScript e SCSS. O projeto também inclui configuração de Angular SSR.

## Requisitos

- Node.js compatível com Angular 19 (Node.js 20.11 ou superior recomendado).
- npm, instalado com o Node.js.

## Instalar dependências

Execute a partir da pasta `front`:

```powershell
npm ci
```

## Executar localmente

O arquivo `src/environments/environment.ts` não é versionado. Se ainda não existir, crie-o a partir do exemplo:

```powershell
if (-not (Test-Path src\environments\environment.ts)) {
  Copy-Item src\environments\environment.example.ts src\environments\environment.ts
}
```

O exemplo aponta para a API local em `https://localhost:7100/api`. No Visual Studio, inicie a API pelo perfil `https` de `Loja.API`; alternativamente, o perfil `http` atende em `http://localhost:5200`.

Depois execute:

```powershell
npm start
```

O script inicializa um certificado HTTPS de desenvolvimento e inicia o Angular CLI na porta `4200`. O front fica disponível em `https://localhost:4200`; na primeira execução, aceite/confie no certificado local quando solicitado.

O container usa uma configuração de ambiente própria, versionada em `src/environments/environment.docker.ts`: as chamadas `/api` são encaminhadas pelo Nginx à API no Compose. Para iniciar o front em Docker, consulte o [README da raiz](../README.md).

## Build e testes

```powershell
npm run build
npm test
```

`npm run build` gera o build em `dist/loja`. `npm test` executa os testes unitários com Angular/Karma.

Para um build de desenvolvimento contínuo:

```powershell
npm run watch
```

## Estrutura útil

- `src/app/pages`: páginas da loja, conta de cliente e administração.
- `src/app/components`: componentes reutilizáveis.
- `src/app/services`: serviços compartilhados e integração com a API.
- `src/environments`: configurações de ambiente local, exemplo e Docker.
- `public` e `src/assets`: recursos estáticos e imagens.
