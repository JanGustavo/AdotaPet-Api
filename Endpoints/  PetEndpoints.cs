using AdotaPet.Api.Data;
using AdotaPet.Api.Models;

namespace AdotaPet.Api.Endpoints;

public static class PetEndpoints
{
    public static async Task MapPetEndpoints(this WebApplication app)
    {
        app.MapPost("/pets", async (
            HttpRequest request,
            AppDbContext db
            ) =>
        {
            //ler o dto do corpo
            var form = await request.ReadFormAsync();

            //mapear os campos
            var pet = new Pet
            {
                Name = form["name"]!,
                Species = form["species"]!,
                Breed = form["breed"],
                Description = form["description"],
            };

            if (int.TryParse(form["age"], out var age))
                pet.Age = age;

            //salvar pet
            db.Pets.Add(pet);
            await db.SaveChangesAsync();

            foreach (var file in form.Files)
            {
                if (file.Length > 0)
                {
                    // 1. Define o caminho da pasta de uploads
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    // 2. A MÁGICA: Se a pasta não existir, cria ela agora!
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // 3. Define o nome do arquivo (Corrigi o Guid.NewGuid() aqui também)
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadPath, fileName);

                    // 4. Salva o arquivo
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // 5. Salva o link no banco
                    db.PetPhotos.Add(new PetPhoto
                    {
                        PetId = pet.Id,
                        Url = $"/uploads/{fileName}"
                    });
                }
            }

            await db.SaveChangesAsync();

            return Results.Ok(pet);
        })

.RequireAuthorization();
    }
}