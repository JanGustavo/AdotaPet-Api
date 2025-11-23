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

        //Get -> pegar o usuário logado
        app.MapGet("/me", async (AppDbContext db, HttpContext http) =>
        {
            //pegar o userID das claims
            var userIdClaim = http.User.Claims.FirstOrDefault(c => c.Type == "userId");

            if (userIdClaim is null) return Results.Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            //buscar usuário no banco
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

        //Post -> criar usuário
        app.MapPost("/users", async (AppDbContext db, UserRegisterDto dto) =>
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                Name = dto.Name,
                UserType = dto.UserType
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

        //faz a solicitação de accesso, se conseguir vai retornar um token o qual vai buscar o id no banco e gerar o token
        app.MapPost("/login", async (AppDbContext db, JwtService jwt, UserLoginDto dto) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null) return Results.BadRequest("Usuário não encontrado");

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordValid) return Results.BadRequest("Senha incorreta");

            var token = jwt.GenerateToken(user.Id);
            return Results.Ok(new { token });
        });
    }
}