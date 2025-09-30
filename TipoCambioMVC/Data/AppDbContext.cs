using Microsoft.EntityFrameworkCore;
using TipoCambioMVC.Models;

namespace TipoCambioMVC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<DivisaUsuario> DivisasUsuario => Set<DivisaUsuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Rol
        modelBuilder.Entity<Rol>(e =>
        {
            e.Property(p => p.IdRol).ValueGeneratedOnAdd();
        });

        // Usuario
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasOne(u => u.Rol)
             .WithMany(r => r.Usuarios)
             .HasForeignKey(u => u.IdRol)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(u => u.IdUsuario).IsUnique();
        });

        //DivisaUsuario
        modelBuilder.Entity<DivisaUsuario>(e =>
        {
            e.Property(p => p.Id).ValueGeneratedOnAdd();
            e.Property(p => p.Codigo).HasMaxLength(5).IsRequired();

            e.HasOne(x => x.Usuario)
             .WithMany(u => u.DivisaUsuarios)
             .HasForeignKey(x => x.IdUsuario)
             .OnDelete(DeleteBehavior.Cascade);

            // Solo una principal por usuario
            e.HasIndex(x => x.IdUsuario)
             .IsUnique()
             .HasFilter("[IsPrincipal] = 1");
        });

        base.OnModelCreating(modelBuilder);
    }
}
