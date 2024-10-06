using System.Reflection;
using FonTech.DAL.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace FonTech.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.AddInterceptors( new DataInterceptor() );
        base.OnConfiguring(options );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<>()
       // base.OnModelCreating(modelBuilder);
       modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    }
}