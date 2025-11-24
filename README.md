
````markdown
# 🐾 AdotaPet API

> API RESTful desenvolvida em .NET Core para a plataforma de adoção de animais **AdotaPet**, com mecânica de navegação estilo Tinder/TikTok.

![Badge .NET](https://img.shields.io/badge/.NET%20Core-8.0-purple)
![Badge License](https://img.shields.io/github/license/JanGustavo/AdotaPet-Api)
![Badge Status](https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow)

## 📋 Sobre o Projeto

O **AdotaPet** é uma plataforma inovadora que conecta adotantes a pets disponíveis para adoção. O backend fornece uma API segura e escalável, implementando autenticação JWT, criptografia de senhas e uma lógica de feed personalizado baseado em interações.

## 🚀 Tecnologias Utilizadas

* **[.NET Core 8.0](https://dotnet.microsoft.com/)** - Framework principal
* **[Entity Framework Core](https://learn.microsoft.com/pt-br/ef/core/)** - ORM para persistência de dados
* **SQLite** - Banco de dados relacional leve (ideal para desenvolvimento)
* **BCrypt.Net** - Biblioteca para criptografia segura de senhas
* **JWT Bearer** - Padrão para autenticação e autorização
* **Swagger/OpenAPI** - Documentação interativa e teste da API

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas, utilizando o padrão **Minimal APIs** do .NET, focando em simplicidade e performance.

```text
AdotaPet-Api/
├── Endpoints/        # Camada de apresentação (rotas e lógica de requisição/resposta)
├── Services/         # Camada de lógica de negócio (regras e orquestração)
├── Models/           # Entidades do domínio (mapeamento do banco de dados)
├── Data/             # Contexto do EF Core e configurações
├── Migrations/       # Histórico de alterações do banco de dados
└── wwwroot/uploads/  # Diretório para armazenamento de arquivos (ex: fotos de pets)
````

## 📦 Pré-requisitos

Antes de começar, você precisará ter instalado em sua máquina:

  * [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
  * [Git](https://git-scm.com/)

## ⚙️ Configuração e Instalação

### 1\. Clone o repositório

```bash
git clone [https://github.com/JanGustavo/AdotaPet-Api.git](https://github.com/JanGustavo/AdotaPet-Api.git)
cd AdotaPet-Api
```

### 2\. Configuração do Ambiente

O projeto utiliza SQLite (`adotapet.db`). Verifique o arquivo `appsettings.json`.

**⚠️ Importante:** Por segurança, altere o valor da chave `Jwt:Key` para uma string única, longa e complexa no seu ambiente local.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=adotapet.db"
  },
  "Jwt": {
    "Key": "SuaChaveSecretaMuitoLongaEComplexaParaAssinarOJWT"
  }
}
```

### 3\. Restaure as dependências e Banco de Dados

```bash
# Baixar pacotes necessários
dotnet restore

# Aplicar as migrações para criar/atualizar o banco de dados
dotnet ef database update
```

### 4\. Execute a aplicação

```bash
dotnet run
```

A API estará disponível em: `https://localhost:5001` (ou conforme configurado no seu `launchSettings.json`).

## 📚 Documentação da API

Após iniciar a aplicação, acesse a documentação interativa completa através do Swagger:
👉 **[http://localhost:5001/swagger](https://www.google.com/search?q=http://localhost:5001/swagger)**

### Principais Endpoints

| Método | Endpoint | Descrição | Autenticação? |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | Cadastro de novo usuário | ❌ Não |
| `POST` | `/api/v1/auth/login` | Login e geração de token JWT | ❌ Não |
| `GET` | `/api/v1/pets/feed` | Lista de pets para o feed (paginado) | ✅ Sim |
| `POST` | `/api/v1/pets/{id}/like` | Registrar interesse em um pet | ✅ Sim |
| `POST` | `/api/v1/pets/{id}/dislike` | Descartar um pet | ✅ Sim |
| `GET` | `/api/v1/users/me` | Obter perfil do usuário logado | ✅ Sim |

## 🔐 Autenticação

A API utiliza **JWT (JSON Web Token)**. Para acessar as rotas protegidas (marcadas com "Sim" acima), você deve enviar o token no cabeçalho da requisição:

```http
Authorization: Bearer {seu_token_aqui}
```

## 🗄️ Estrutura do Banco de Dados

O banco SQLite é gerenciado pelo EF Core e possui as seguintes entidades principais:

  * **User:** Dados de adotantes e ONGs.
  * **Pet:** Informações dos animais disponíveis (Nome, Raça, Idade, etc).
  * **PetPhoto:** Urls das fotos dos pets.
  * **UserInteraction:** Tabela de junção que registra Likes/Dislikes para não repetir pets no feed.

## 🛠️ Scripts Úteis (Entity Framework)

```bash
# Criar nova migração (caso altere os Models)
dotnet ef migrations add NomeDaMigracao

# Aplicar migrações pendentes ao banco
dotnet ef database update

# Limpar build e recompilar (caso dê erro de cache)
dotnet clean && dotnet build
```

## 🤝 Contribuindo

Contribuições são bem-vindas\!

1.  Faça um **fork** do projeto.
2.  Crie uma **branch** para sua feature (`git checkout -b feature/minha-feature`).
3.  Envie suas alterações (`git commit -m 'Adiciona nova feature'`).
4.  Abra um **Pull Request**.

## 📄 Licença

Este projeto está sob a licença **MIT**. Veja o arquivo [LICENSE](https://www.google.com/search?q=LICENSE) para mais detalhes.

## 👥 Autor

Feito com ❤️ por **Jan Gustavo**.

[](https://www.google.com/search?q=https://github.com/JanGustavo)

```
```
