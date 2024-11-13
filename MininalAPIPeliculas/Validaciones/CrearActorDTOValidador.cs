using FluentValidation;
using MininalAPIPeliculas.DTOs;

namespace MininalAPIPeliculas.Validaciones
{
	public class CrearActorDTOValidador : AbstractValidator<CrearActorDTO>
	{
        public CrearActorDTOValidador()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                .MaximumLength(150).WithMessage(Utilidades.MaximoLengthMensaje);

            var fechaMinima = new DateTime(1900, 1, 1);

            RuleFor(x => x.FechaNacimiento)
                .GreaterThanOrEqualTo(fechaMinima).WithMessage(Utilidades.GreaterThanOrEqualToMensaje(fechaMinima));
        }
    }
}
