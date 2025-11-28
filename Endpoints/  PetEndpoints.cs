using AdotaPet.Api.Data;
using AdotaPet.Api.Models;
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
                Name = form["name"]!,
                Species = form["species"]!,
                Breed = form["breed"],
                Description = form["description"],
                UserId = Guid.Parse(userIdClaim.Value) // Vincula ao usuário logado
            };

            if (int.TryParse(form["age"], out var age)) pet.Age = age;

            db.Pets.Add(pet);
            await db.SaveChangesAsync();

            // Lógica de Upload de Foto
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

            // 2. LÓGICA DO DEMO (Hardcode Seguro)
            if (pets.Any())
            {
                // Pega o primeiro pet da lista para transformar no Thor
                var demoPet = pets[0];

                // Força os dados para a apresentação
                demoPet.Name = "Thor";
                demoPet.Description = "Cachorro amigável para adoção urgente! Muito brincalhão.";

                // Se tiver dono, força o telefone. Se não tiver, ignora para não crashar.
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
                OwnerName = p.User?.Name,       // Manda o nome do dono
                OwnerPhone = p.User?.Phone,     // Manda o telefone (que forçamos acima)

                // Transforma a lista de objetos Photo em lista de Strings (URLs)
                Photos = p.Photos.Select(photo => photo.Url).ToList()
            })
            .OrderBy(x => Guid.NewGuid()); // Embaralha para parecer dinâmico

            return Results.Ok(response);
        }).RequireAuthorization();
    }
}