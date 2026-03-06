# DiihTemplate

Template .NET 10 com Clean Architecture e DDD para criacao rapida de APIs.

## Instalacao

### Via diretorio local

```bash
git clone https://github.com/sua-org/diihtemplate.git
dotnet new install ./diihtemplate
```

### Via NuGet (feed privado)

```bash
# Adicionar o feed (exemplo com GitHub Packages)
dotnet nuget add source https://nuget.pkg.github.com/SUA-ORG/index.json \
  --name github --username SEU-USER --password GH_TOKEN

# Instalar
dotnet new install DiihVeloper.Template
```

### Via arquivo .nupkg

```bash
dotnet new install ./DiihVeloper.Template.1.0.0.nupkg
```

## Uso

### Criacao basica (padrao: Postgres, sem extras)

```bash
dotnet new diihtemplate -n MeuProjeto
```

### Escolhendo o banco de dados

```bash
# PostgreSQL (padrao)
dotnet new diihtemplate -n MeuProjeto --DATABASE Postgres

# SQL Server
dotnet new diihtemplate -n MeuProjeto --DATABASE SqlServer
```

### Habilitando features opcionais

```bash
# Com envio de emails (SMTP via MailKit)
dotnet new diihtemplate -n MeuProjeto --SMTP true

# Com mensageria RabbitMQ
dotnet new diihtemplate -n MeuProjeto --RABBITMQ true

# Com armazenamento de arquivos (local)
dotnet new diihtemplate -n MeuProjeto --STORAGE true
```

### Exemplo completo

```bash
dotnet new diihtemplate -n MeuProjeto \
  --DATABASE Postgres \
  --SMTP true \
  --RABBITMQ true \
  --STORAGE true
```

## Parametros

| Parametro    | Tipo   | Padrao     | Descricao                                      |
|--------------|--------|------------|-------------------------------------------------|
| `-n`         | string | —          | Nome do projeto (substitui namespaces e pastas) |
| `--DATABASE` | choice | `Postgres` | Banco de dados: `Postgres` ou `SqlServer`       |
| `--SMTP`     | bool   | `false`    | Envio de emails via SMTP (MailKit)               |
| `--RABBITMQ` | bool   | `false`    | Mensageria com RabbitMQ                          |
| `--STORAGE`  | bool   | `false`    | Armazenamento de arquivos (filesystem local)     |

## Estrutura gerada

```
MeuProjeto/
├── src/
│   ├── MeuProjeto.Api/                # Controllers, Program.cs, appsettings.json
│   ├── MeuProjeto.Application/        # Services, Mapster mappings, validators
│   ├── MeuProjeto.Application.Contracts/ # DTOs compartilhados
│   ├── MeuProjeto.Core/               # Base classes, interfaces, middlewares
│   ├── MeuProjeto.Data/               # EF Core DbContext, configuracoes
│   ├── MeuProjeto.Domain/             # Entidades de negocio
│   ├── MeuProjeto.Domain.Shared/      # Enums, constantes
│   └── MeuProjeto.Infra/             # Servicos externos (email, messaging, storage)
├── tests/
│   └── MeuProjeto.Tests/             # Testes de integracao (xUnit)
├── .claude/                           # Claude Code config (CLAUDE.md + skills)
├── docker-compose.yml
└── MeuProjeto.sln
```

## Primeiros passos apos criar o projeto

```bash
cd MeuProjeto

# Restaurar dependencias
dotnet restore

# Configurar connection string no appsettings.json
# (editar src/MeuProjeto.Api/appsettings.json)

# Rodar
dotnet run --project src/MeuProjeto.Api

# Rodar testes
dotnet test

# Docker (com Postgres)
docker compose up --build
```

## Features opcionais em detalhe

### SMTP (--SMTP true)

Adiciona suporte a envio de emails via MailKit.

**Arquivos gerados:**
- `Core/Services/IEmailSender.cs` — interface + modelos (`EmailMessage`, `EmailAttachment`)
- `Infra/Email/SmtpEmailSender.cs` — implementacao SMTP
- `Infra/Email/EmailSettings.cs` — configuracao

**Configuracao no `appsettings.json`:**

```json
{
  "Email": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "seu-usuario",
    "Password": "sua-senha",
    "FromEmail": "noreply@example.com",
    "FromName": "MeuProjeto",
    "UseSsl": true
  }
}
```

**Uso:**

```csharp
public class MeuService(IEmailSender emailSender)
{
    public async Task EnviarBoasVindas(string email, string nome)
    {
        await emailSender.SendAsync(new EmailMessage
        {
            To = email,
            Subject = "Bem-vindo!",
            Body = $"<h1>Ola, {nome}!</h1>",
            IsHtml = true
        });
    }
}
```

### RabbitMQ (--RABBITMQ true)

Adiciona suporte a mensageria com RabbitMQ.

**Arquivos gerados:**
- `Core/Services/IMessageBroker.cs` — interface publish/subscribe
- `Infra/Messaging/RabbitMqMessageBroker.cs` — implementacao com RabbitMQ.Client v7
- `Infra/Messaging/RabbitMqSettings.cs` — configuracao

**Configuracao no `appsettings.json`:**

```json
{
  "RabbitMq": {
    "HostName": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "DefaultExchange": ""
  }
}
```

**Uso:**

```csharp
// Publicar
public class PedidoService(IMessageBroker broker)
{
    public async Task CriarPedido(Pedido pedido)
    {
        // ... salvar pedido
        await broker.PublishAsync(new PedidoCriadoEvent { PedidoId = pedido.Id });
    }
}

// Consumir (em um HostedService, por exemplo)
await broker.SubscribeAsync<PedidoCriadoEvent>("pedidos-criados", async (msg, ct) =>
{
    // processar mensagem
});
```

### Storage (--STORAGE true)

Adiciona suporte a armazenamento de arquivos no filesystem local.

**Arquivos gerados:**
- `Core/Services/IFileStorage.cs` — interface (upload, download, delete, exists)
- `Infra/Storage/LocalFileStorage.cs` — implementacao local
- `Infra/Storage/StorageSettings.cs` — configuracao

**Configuracao no `appsettings.json`:**

```json
{
  "Storage": {
    "BasePath": "./uploads"
  }
}
```

**Uso:**

```csharp
public class ArquivoService(IFileStorage storage)
{
    public async Task<string> Upload(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return await storage.UploadAsync(stream, file.FileName, folder: "documentos");
    }

    public async Task<Stream> Download(string path)
    {
        return await storage.DownloadAsync(path);
    }
}
```

## Arquitetura

```
Api → Application → Domain ← Data/Infra
              ↕
             Core (compartilhado entre todas as camadas)
```

| Camada                  | Responsabilidade                                          |
|-------------------------|-----------------------------------------------------------|
| `Api`                   | Controllers, middlewares, Program.cs                      |
| `Application`           | Application services, mappings (Mapster), validators      |
| `Application.Contracts` | DTOs, interfaces de servicos compartilhados               |
| `Core`                  | Base classes, interfaces genericas, repositorios, eventos |
| `Domain`                | Entidades de negocio                                      |
| `Domain.Shared`         | Enums, constantes compartilhadas                          |
| `Data`                  | EF Core DbContext, configuracoes de banco                 |
| `Infra`                 | Implementacoes de servicos externos                       |

## Claude Code

O projeto gerado inclui configuracao para o [Claude Code](https://claude.ai/code):

- **`.claude/CLAUDE.md`** — contexto do projeto adaptado automaticamente (banco, features habilitadas, namespaces)
- **Skills disponiveis:**
  - `/scaffold-crud [Entity] [KeyType]` — gera entity, DTOs, mapping, validator, controller e DbSet
  - `/add-domain-event [Name] [pre|post]` — cria domain event com handler
  - `/add-validator [DtoName]` — cria FluentValidation validator
  - `/add-service [Name]` — cria application service com interface e DI
  - `/add-migration [Name]` — cria migration EF Core

## Desinstalacao

```bash
dotnet new uninstall DiihVeloper.Template
# ou, se instalou via diretorio:
dotnet new uninstall /caminho/para/diihtemplate
```
