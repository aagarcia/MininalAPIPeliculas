using FluentValidation;
using MininalAPIPeliculas.DTOs;

namespace MininalAPIPeliculas.Validaciones
{
    public class EditarClaimDTOValidador : AbstractValidator<EditarClaimDTO>
    {
        public EditarClaimDTOValidador()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(Utilidades.CampoRequeridoMensaje)
                                 .MaximumLength(256).WithMessage(Utilidades.MaximoLengthMensaje)
                                 .EmailAddress().WithMessage(Utilidades.EmailMensaje);
        }
    }
}
