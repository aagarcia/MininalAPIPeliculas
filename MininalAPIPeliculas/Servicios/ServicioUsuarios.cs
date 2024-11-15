using Microsoft.AspNetCore.Identity;

namespace MininalAPIPeliculas.Servicios
{
    public class ServicioUsuarios : IServicioUsuarios
    {
        private readonly IHttpContextAccessor httpContextAccesor;
        private readonly UserManager<IdentityUser> userManager;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccesor,
                                UserManager<IdentityUser> userManager)
        {
            this.httpContextAccesor = httpContextAccesor;
            this.userManager = userManager;
        }

        public async Task<IdentityUser?> ObtenerUsuario()
        {
            var emailClaim = httpContextAccesor.HttpContext!.User.Claims.Where(x => x.Type == "email").FirstOrDefault();

            if (emailClaim is null)
            {
                return null;
            }

            var email = emailClaim.Value;
            var usuarioEncontrado = await userManager.FindByEmailAsync(email);
            return usuarioEncontrado;
        }
    }
}
