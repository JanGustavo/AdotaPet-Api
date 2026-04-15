using AdotaPet.Api.Data;

using Microsoft.EntityFrameworkCore;

using AdotaPet.Api.Endpoints;
using AdotaPet.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // URL do Front
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// --- CONFIGURA��O DE SERVI�OS ---

builder.Services.AddDbContext<AppDbContext>(options =>

    options.UseSqlite("Data Source=adotapet.db"));

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AdotaPet API", Version = "v1" });
});

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

builder.Services.AddAuthorization(); // Servi�o de permiss�o

builder.Services.ConfigureHttpJsonOptions(options =>

{
    options.SerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();



// --- CONFIGURA��O DE PIPELINE (Middleware) ---

app.UseStaticFiles(); // Para servir as fotos salvas



if (app.Environment.IsDevelopment())

{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AdotaPet API v1");
        options.RoutePrefix = "swagger";
    });
}

// Rota padrão na raiz
app.MapGet("/", () => Results.Content(
    "<h1>🐾 AdotaPet API</h1><p>API para plataforma de adoção de animais</p><p><a href='/swagger'>Documentação Swagger</a></p>",
    "text/html"))
  .WithName("Home")
  .WithOpenApi();

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp"); //avisa 



app.UseAuthentication(); // 1 verifica quem �

app.UseAuthorization();  // 2 verifica se pode entrar



// --- SEUS ENDPOINTS ---

await app.MapUserEndpoints();

app.MapPetEndpoints();

app.Run();