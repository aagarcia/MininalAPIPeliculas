using FluentValidation;
using MininalAPIPeliculas.DTOs;

namespace MininalAPIPeliculas.Validaciones
{
    public class CrearPeliculaDTOValidador : AbstractValidator<CrearPeliculaDTO>
    {
        public CrearPeliculaDTOValidador()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(150).WithMessage(Utilidades.MaximoLengthMensaje);
        }
    }
}
