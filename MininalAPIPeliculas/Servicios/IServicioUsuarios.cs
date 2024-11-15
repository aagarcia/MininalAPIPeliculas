using Microsoft.AspNetCore.Identity;

namespace MininalAPIPeliculas.Servicios
{
    public interface IServicioUsuarios
    {
        Task<IdentityUser?> ObtenerUsuario();
    }
}