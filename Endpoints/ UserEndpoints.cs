using AdotaPet.Api.Data;
using AdotaPet.Api.Models;
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

        //Post -> criar usuário simples
        app.MapPost("/users", async (AppDbContext db, User user) =>
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/users/{user.Id}", user);
        });
    }

}