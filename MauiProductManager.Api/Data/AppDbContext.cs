using Microsoft.EntityFrameworkCore;
using MauiProductManager.Api.Models;

namespace MauiProductManager.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
}
