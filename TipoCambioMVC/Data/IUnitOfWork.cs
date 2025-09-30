using TipoCambioMVC.Models;

namespace TipoCambioMVC.Data;

public interface IUnitOfWork
{
    IRepository<DivisaUsuario> DivisasUsuario { get; }
    IRepository<Rol> Roles { get; }
    IRepository<Usuario> Usuarios { get; }


    Task<int> SaveChangesAsync();
}
