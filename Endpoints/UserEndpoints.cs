using AdotaPet.Api.Data;
using AdotaPet.Api.Models;
using AdotaPet.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdotaPet.Api.Endpoints;

static class UserEndpoints
{
    public static async Task MapUserEndpoints(this WebApplication app)
    {
        //Get usuarios
        app.MapGet("/users", async (AppDbContext db) =>
        {
            var users = await db.Users.ToListAsync();
            return Results.Ok(users);
        });

        //Get -> pegar o usu�rio logado
        app.MapGet("/me", async (AppDbContext db, HttpContext http) =>
        {
            //pegar o userID das claims
            var userIdClaim = http.User.Claims.FirstOrDefault(c => c.Type == "userId");

            if (userIdClaim is null) return Results.Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            //buscar usu�rio no banco
            var user = await db.Users.FindAsync(userId);

            if (user is null) return Results.NotFound();

            return Results.Ok(new
            {
                user.Id,
                user.Email,
                user.Name,
                user.UserType
            });
        })
         .RequireAuthorization();

        //Post -> criar usu�rio
        app.MapPost("/users", async (AppDbContext db, UserRegisterDto dto) =>
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                Name = dto.Name,
                UserType = dto.UserType,
                Phone = dto.Phone,
                HasWhatsapp = dto.HasWhatsapp,
                PetsRegistered = dto.PetsRegistered
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created($"/users/{user.Id}", new
            {
                user.Id,
                user.Email,
                user.Name,
                user.UserType
            });
        });

        //faz a solicita��o de accesso, se conseguir vai retornar um token o qual vai buscar o id no banco e gerar o token
        app.MapPost("/login", async (AppDbContext db, JwtService jwt, UserLoginDto dto) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null) return Results.BadRequest("Usu�rio n�o encontrado");

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordValid) return Results.BadRequest("Senha incorreta");

            var token = jwt.GenerateToken(user.Id);
            return Results.Ok(new { token });
        });

        // deletar usu�rio
        app.MapDelete("/users/{id}", async (AppDbContext db, HttpContext http, Guid id) =>
        {
            // Pega o ID do usuário do token
            var userIdClaim = http.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim is null) return Results.Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            // Valida se está deletando a própria conta
            if (userId != id) return Results.Forbid();

            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound("Usuário não encontrado");

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Results.Ok("Usuário deletado com sucesso");
        })
        .RequireAuthorization();

        app.MapPatch("/users/{id}", async (AppDbContext db, HttpContext http, Guid id, UpdateUserDto dto) =>
        {
            // Pega o ID do usuário do token
            var userIdClaim = http.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim is null) return Results.Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            // Valida se está atualizando a própria conta
            if (userId != id) return Results.Forbid();

            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound("Usuário não encontrado");

            // Atualiza os campos permitidos
            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.HasWhatsapp = dto.HasWhatsapp;
            user.PetsRegistered = dto.PetsRegistered;

            await db.SaveChangesAsync();

            return Results.Ok("Usuário atualizado com sucesso");

        })
        .RequireAuthorization();

        //atualizar informações


    }
}