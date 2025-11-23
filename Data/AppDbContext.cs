using AdotaPet.Api.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AdotaPet.Api.Data;

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) { }

    public DbSet<User> Users => Set<User>();

    public DbSet<Pet> Pets => Set<Pet>();

    public DbSet<PetPhoto> PetPhotos => Set<PetPhoto>(); 

}