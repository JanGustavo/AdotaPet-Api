# Dependências do Projeto AdotaPet.Api

## Framework
- **.NET 10.0** (net10.0)

## Pacotes NuGet

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| BCrypt.Net-Next | 4.0.3 | Biblioteca para hash seguro de senhas |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.0 | Autenticação JWT no ASP.NET Core |
| Microsoft.AspNetCore.OpenApi | 10.0.0 | Suporte a OpenAPI/Swagger |
| Microsoft.EntityFrameworkCore | 10.0.0 | ORM para acesso a banco de dados |
| Microsoft.EntityFrameworkCore.Design | 10.0.0 | Ferramentas de design para Entity Framework |
| Microsoft.EntityFrameworkCore.Sqlite | 10.0.0 | Provider SQLite para Entity Framework |
| System.IdentityModel.Tokens.Jwt | 8.15.0 | Manipulação de tokens JWT |

## Instalação das Dependências

Para instalar/restaurar as dependências, execute:

```bash
dotnet restore
```

## Build e Execução

Para compilar o projeto:
```bash
dotnet build
```

Para executar o projeto:
```bash
dotnet run
```

## Configuração necessária

1. **JWT Secret Key**: Configurar a chave secreta no `Program.cs` ou em variáveis de ambiente
2. **Banco de Dados**: SQLite será criado automaticamente em `adotapet.db`
3. **CORS**: Configurado para aceitar requisições do `http://localhost:4200`
