using AdotaPet.Api.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AdotaPet.Api.Data;

class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique(); //torna o email unico
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Pet> Pets => Set<Pet>();

    public DbSet<PetPhoto> PetPhotos => Set<PetPhoto>(); 
}