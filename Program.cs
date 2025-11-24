using AdotaPet.Api.Data;

using Microsoft.EntityFrameworkCore;

using AdotaPet.Api.Endpoints;

using AdotaPet.Api.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

using System.Text;



var builder = WebApplication.CreateBuilder(args);



// --- CONFIGURAÇÃO DE SERVIÇOS ---

builder.Services.AddDbContext<AppDbContext>(options =>

    options.UseSqlite("Data Source=adotapet.db"));



builder.Services.AddOpenApi();



var secretKey = "sua-chave-ultra-secreta-32chars-no-minimo";

builder.Services.AddSingleton(new JwtService(secretKey));



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)

    .AddJwtBearer(options =>

    {

        options.TokenValidationParameters = new TokenValidationParameters

        {

            ValidateIssuer = false,

            ValidateAudience = false,

            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            ClockSkew = TimeSpan.Zero

        };

    });



builder.Services.AddAuthorization(); // Serviço de permissão



builder.Services.ConfigureHttpJsonOptions(options =>

{

    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;

});



var app = builder.Build();



// --- CONFIGURAÇÃO DE PIPELINE (Middleware) ---

app.UseStaticFiles(); // Para servir as fotos salvas



if (app.Environment.IsDevelopment())

{

    app.MapOpenApi();

}



app.UseHttpsRedirection();



// A ORDEM AQUI É SAGRADA:

app.UseAuthentication(); // 1 verifica quem é

app.UseAuthorization();  // 2 verifica se pode entrar



// --- SEUS ENDPOINTS ---

await app.MapUserEndpoints();

await app.MapPetEndpoints();



app.Run();