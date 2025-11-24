using AdotaPet.Api.Data;
using AdotaPet.Api.Models;
using Microsoft.EntityFrameworkCore; 

namespace AdotaPet.Api.Endpoints;

public static class FeedEndpoints
{
    public static void FeedPetEndpoints(this WebApplication app)
    {
        app.MapGet("/feed", async (AppDbContext db) =>
        {
            var pets = await db.Pets
                .Include(pets => pets.Photos)
                .ToListAsync();

            var response = pets.Select(p => new PetResponseDto
            { Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Species = p.Species,
                Breed = p.Breed,
                Photos = p.Photos.Select(photo => photo.Url).ToList()
            });

            return Results.Ok(response);
        });
    }
}
