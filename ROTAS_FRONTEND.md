# Rotas atuais da API (guia para o Frontend)

Este arquivo documenta as rotas que existem hoje no backend para facilitar a integracao com o frontend.

## Base URL

- Local (HTTP): `http://localhost:{porta}`
- Local (HTTPS): `https://localhost:{porta}`
- Swagger: `/swagger`

> A porta exata depende do profile de execucao (veja `Properties/launchSettings.json`).

## CORS atual

A API esta configurada para aceitar chamadas do frontend em:

- `http://localhost:4200`

## Autenticacao

A API usa JWT Bearer.

- Header esperado nas rotas protegidas:

```http
Authorization: Bearer <token>
```

- O token e retornado pela rota `POST /login`.

---

## 1) Home

### GET `/`

- Publica
- Retorna HTML simples com link para o Swagger.

Resposta (exemplo):

```html
<h1>🐾 AdotaPet API</h1>
<p>API para plataforma de adoção de animais</p>
<p><a href='/swagger'>Documentação Swagger</a></p>
```

---

## 2) Usuarios

### GET `/users`

- Publica
- Retorna todos os usuarios cadastrados no banco.

Resposta:

- `200 OK` com array de objetos `User`.

Observacao importante:

- Como hoje a rota retorna a entidade `User` diretamente, ela pode expor campos sensiveis como `PasswordHash`.

---

### GET `/me`

- Protegida (`RequireAuthorization`)
- Retorna dados basicos do usuario autenticado.

Resposta de sucesso (`200 OK`):

```json
{
  "id": "guid",
  "email": "usuario@email.com",
  "name": "Nome",
  "userType": 1
}
```

Possiveis erros:

- `401 Unauthorized` (token ausente/invalido)
- `404 Not Found` (usuario nao encontrado)

---

### POST `/users`

- Publica
- Cria novo usuario.

Body (`application/json`):

```json
{
  "email": "usuario@email.com",
  "password": "123456",
  "name": "Nome",
  "userType": 1,
  "phone": "55999999999",
  "hasWhatsapp": true,
  "petsRegistered": 0
}
```

Campos:

- `email`: string
- `password`: string
- `name`: string
- `userType`: number (enum)
  - `1 = Adotante`
  - `2 = Ong`
  - `3 = Admin`
- `phone`: string
- `hasWhatsapp`: boolean
- `petsRegistered`: number

Resposta de sucesso (`201 Created`):

```json
{
  "id": "guid",
  "email": "usuario@email.com",
  "name": "Nome",
  "userType": 1
}
```

---

### POST `/login`

- Publica
- Autentica usuario e retorna token JWT.

Body (`application/json`):

```json
{
  "email": "usuario@email.com",
  "password": "123456"
}
```

Resposta de sucesso (`200 OK`):

```json
{
  "token": "jwt-token"
}
```

Possiveis erros:

- `400 Bad Request` com mensagem:
  - `"Usuario nao encontrado"`
  - `"Senha incorreta"`

---

### PATCH `/users/{id}`

- Protegida (`RequireAuthorization`)
- Atualiza dados do proprio usuario (o `id` da URL deve ser o mesmo do token).

Body (`application/json`):

```json
{
  "name": "Novo Nome",
  "email": "novo@email.com",
  "phone": "55999999999",
  "hasWhatsapp": true,
  "petsRegistered": 2
}
```

Resposta de sucesso:

- `200 OK` com string: `"Usuario atualizado com sucesso"`

Possiveis erros:

- `401 Unauthorized`
- `403 Forbidden` (tentando editar outro usuario)
- `404 Not Found` (usuario nao encontrado)

---

### DELETE `/users/{id}`

- Protegida (`RequireAuthorization`)
- Remove o proprio usuario (o `id` da URL deve ser o mesmo do token).

Resposta de sucesso:

- `200 OK` com string: `"Usuario deletado com sucesso"`

Possiveis erros:

- `401 Unauthorized`
- `403 Forbidden` (tentando deletar outro usuario)
- `404 Not Found` (usuario nao encontrado)

---

## 3) Pets

### POST `/pets`

- Protegida (`RequireAuthorization`)
- Cria pet e permite upload de fotos.
- Content-Type: `multipart/form-data`

Campos de formulario:

- `name` (string, obrigatorio)
- `species` (string, obrigatorio)
- `breed` (string, opcional)
- `description` (string, opcional)
- `age` (number, opcional)
- arquivos: qualquer chave de arquivo no form (a API percorre `form.Files`)

Exemplo em frontend (FormData):

```ts
const form = new FormData();
form.append('name', 'Thor');
form.append('species', 'Cachorro');
form.append('breed', 'SRD');
form.append('description', 'Brincalhao');
form.append('age', '3');
form.append('photo1', fileInput.files[0]);
```

Resposta de sucesso:

- `200 OK` com objeto `Pet` criado (entidade do banco).

Observacao:

- As imagens sao salvas em `wwwroot/uploads` e as URLs ficam no formato `/uploads/{nomeArquivo}`.

---

### GET `/feed`

- Protegida (`RequireAuthorization`)
- Retorna lista de pets para exibicao no feed, no formato `PetResponseDto`.
- A lista e embaralhada no backend (`OrderBy(Guid.NewGuid())`).

Resposta (`200 OK`):

```json
[
  {
    "id": "guid",
    "name": "Thor",
    "species": "Cachorro",
    "breed": "SRD",
    "description": "Cachorro amigavel para adocao urgente!",
    "age": 3,
    "ownerName": "Nome dono",
    "ownerPhone": "55999999999",
    "photos": [
      "/uploads/arquivo1.jpg",
      "/uploads/arquivo2.jpg"
    ]
  }
]
```

Campos de `PetResponseDto`:

- `id`: guid
- `name`: string
- `species`: string
- `breed`: string | null
- `description`: string | null
- `age`: number | null
- `ownerName`: string | null
- `ownerPhone`: string | null
- `photos`: string[]

---

## Fluxo recomendado para o frontend

1. Criar conta com `POST /users` (ou usar usuario existente).
2. Fazer login em `POST /login` e guardar `token`.
3. Enviar `Authorization: Bearer <token>` nas rotas protegidas.
4. Carregar perfil com `GET /me`.
5. Carregar cards com `GET /feed`.
6. Cadastrar pet com `POST /pets` usando `FormData`.

---

## Status de protecao por rota

Publicas:

- `GET /`
- `GET /users`
- `POST /users`
- `POST /login`

Protegidas (JWT):

- `GET /me`
- `PATCH /users/{id}`
- `DELETE /users/{id}`
- `POST /pets`
- `GET /feed`
