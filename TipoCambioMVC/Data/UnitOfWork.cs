using TipoCambioMVC.Models;

namespace TipoCambioMVC.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _ctx;

    public UnitOfWork(AppDbContext ctx)
    {
        _ctx = ctx;
        DivisasUsuario = new Repository<DivisaUsuario>(_ctx);
        Roles = new Repository<Rol>(_ctx);
        Usuarios = new Repository<Usuario>(_ctx);
    }

    public IRepository<DivisaUsuario> DivisasUsuario { get; }
    public IRepository<Rol> Roles { get; }
    public IRepository<Usuario> Usuarios { get; }
    public Task<int> SaveChangesAsync() => _ctx.SaveChangesAsync();
}
