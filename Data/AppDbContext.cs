using Microsoft.EntityFrameworkCore;
using AdotaPet.Api.Models;

namespace AdotaPet.Api.Data;

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
           
    }

    public DbSet<User> Users => Set<User>();
}