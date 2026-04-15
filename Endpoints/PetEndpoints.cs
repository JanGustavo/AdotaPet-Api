using AdotaPet.Api.Data;
using AdotaPet.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Importante para o .Include e .ToListAsync

namespace AdotaPet.Api.Endpoints;

public static class PetEndpoints
{
    public static void MapPetEndpoints(this WebApplication app)
    {
        // --- CREATE POST (CRIAR PET) ---
        app.MapPost("/pets", async (HttpRequest request, AppDbContext db) =>
        {
            var userIdClaim = request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim is null) return Results.Unauthorized();

            var form = await request.ReadFormAsync();

            var pet = new Pet
            {
                Name = form["name"].ToString(),
                Species = form["species"].ToString(),
                Breed = form["breed"],
                Description = form["description"],
                UserId = Guid.Parse(userIdClaim.Value)
            };

            if (int.TryParse(form["age"], out var age)) pet.Age = age;

            db.Pets.Add(pet);
            await db.SaveChangesAsync();

            // L�gica de Upload de Foto
            foreach (var file in form.Files)
            {
                if (file.Length > 0)
                {
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    db.PetPhotos.Add(new PetPhoto
                    {
                        PetId = pet.Id,
                        Url = $"/uploads/{fileName}"
                    });
                }
            }
            await db.SaveChangesAsync();
            return Results.Ok(pet);
        }).RequireAuthorization();


        // --- GET FEED (COM O TRUQUE DO THOR) ---
        app.MapGet("/feed", async (AppDbContext db) =>
        {
            // 1. Busca os pets e inclui o Dono (User) e as Fotos
            var pets = await db.Pets
                .Include(p => p.User)
                .Include(p => p.Photos)
                .ToListAsync();

            // 2. L�GICA DO DEMO (Hardcode Seguro)
            if (pets.Count > 0)
            {
                // Pega o primeiro pet da lista para transformar no Thor
                var demoPet = pets[0];

                // For�a os dados para a apresenta��o
                demoPet.Name = "Thor";
                demoPet.Description = "Cachorro amig�vel para ado��o urgente! Muito brincalh�o.";

                // Se tiver dono, for�a o telefone. Se n�o tiver, ignora para n�o crashar.
                if (demoPet.User != null)
                {
                    demoPet.User.Phone = "5583998442632"; // O NÚMERO DO WHATSAPP
                    demoPet.User.Name = "Janderson (Dono)";
                }
            }

            // 3. Mapeia para o DTO que o Frontend espera
            var response = pets.Select(p => new PetResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Species = p.Species,
                Breed = p.Breed,
                Description = p.Description,
                OwnerName = p.User?.Name,
                OwnerPhone = p.User?.Phone,
                Photos = p.Photos.Select(photo => photo.Url).ToList()
            })
            .OrderBy(x => Guid.NewGuid());



            return Results.Ok(response);
        }).RequireAuthorization();

        app.MapGet("/me/pets", async (AppDbContext db, HttpContext http) =>
        {
            var userIdClaim = http.User.Claims.FirstOrDefault(c => c.Type == "userId");
            if (userIdClaim is null) return Results.Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            var pets = await db.Pets
                .Include(p => p.User)
                .Include(p => p.Photos)
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var response = pets.Select(p => new PetResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Species = p.Species,
                Breed = p.Breed,
                Description = p.Description,
                OwnerName = p.User?.Name,
                OwnerPhone = p.User?.Phone,
                Photos = p.Photos.Select(photo => photo.Url).ToList()
            });

            return Results.Ok(response);
        }).RequireAuthorization();


        app.MapGet("/pets/{id}", async (Guid id, AppDbContext db) =>
        {
            var pet = await db.Pets
                .Include(p => p.User)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet is null) return Results.NotFound("Pet não encontrado");

            return Results.Ok(pet);
        });
    }
}