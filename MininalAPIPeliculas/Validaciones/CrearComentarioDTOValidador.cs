using FluentValidation;
using MininalAPIPeliculas.DTOs;

namespace MininalAPIPeliculas.Validaciones
{
    public class CrearComentarioDTOValidador : AbstractValidator<CrearComentarioDTO>
    {
        public CrearComentarioDTOValidador()
        {
            RuleFor(x => x.Cuerpo).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje);
        }
    }
}
