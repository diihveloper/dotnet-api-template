---
name: add-migration
description: Cria uma migration EF Core com o nome especificado
argument-hint: [MigrationName]
allowed-tools: Bash, Read, Glob
disable-model-invocation: true
---

# Criar Migration EF Core

Crie uma migration chamada **$ARGUMENTS**.

## Passos

1. Verifique se o projeto compila antes de criar a migration:
   ```bash
   dotnet build DiihTemplate.sln
   ```

2. Crie a migration usando o projeto Data como target e Api como startup:
   ```bash
   dotnet ef migrations add $ARGUMENTS --project src/DiihTemplate.Data --startup-project src/DiihTemplate.Api --output-dir Migrations
   ```

3. Se o comando falhar por falta do tool `dotnet-ef`, instrua o usuário:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. Mostre o arquivo de migration gerado para o usuário revisar.

5. Informe como aplicar:
   ```bash
   dotnet ef database update --project src/DiihTemplate.Data --startup-project src/DiihTemplate.Api
   ```

## Notas

- O nome da migration deve ser PascalCase descritivo (ex: `AddProductTable`, `AddIndexOnEmail`)
- Sempre verifique o build antes de gerar a migration para evitar migrations vazias
- Se houver erros, analise e corrija antes de prosseguir
