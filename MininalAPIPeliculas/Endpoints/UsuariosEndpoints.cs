using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Filtros;
using MininalAPIPeliculas.Utilidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MininalAPIPeliculas.Endpoints
{
	public static class UsuariosEndpoints
	{
		public static RouteGroupBuilder MapUsuarios(this RouteGroupBuilder group)
		{
			group.MapPost("/registrar", Registrar)
				 .AddEndpointFilter<FiltroValidaciones<CredencialesUsuarioDTO>>();
			group.MapPost("login", Login).AddEndpointFilter<FiltroValidaciones<CredencialesUsuarioDTO>>();
			return group;
		}

		static async Task<Results<Ok<string>, BadRequest<IEnumerable<IdentityError>>>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO,
																						[FromServices] UserManager<IdentityUser> userManager,
																						IConfiguration configuration)
		{
			var usuario = new IdentityUser
			{
				UserName = credencialesUsuarioDTO.Email,
				Email = credencialesUsuarioDTO.Email
			};

			var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

			if (resultado.Succeeded)
			{
                /*var respuestaAutenticacion = ConstruirToken(credencialesUsuarioDTO, configuration);
				return TypedResults.Ok(respuestaAutenticacion);*/
                return TypedResults.Ok($"Bienvenido al sistema {credencialesUsuarioDTO.Email}.");
            }
			else 
			{
				return TypedResults.BadRequest(resultado.Errors);
			}
		}

		static async Task<Results<Ok<RespuestaAutenticacionDTO>, BadRequest<string>>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO,
																	                        [FromServices] SignInManager<IdentityUser> signInManager,
																						    [FromServices] UserManager<IdentityUser> userManager,
																						    IConfiguration configuration)
		{
			var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

			if (usuario is null)
			{
				return TypedResults.BadRequest("Login incorrecto");
			}

			var resultado = await signInManager.CheckPasswordSignInAsync(usuario, 
																		 credencialesUsuarioDTO.Password,
																		 lockoutOnFailure: false);

			if (resultado.Succeeded)
			{
				var respuestaAutenticacion = ConstruirToken(credencialesUsuarioDTO, configuration);
				return TypedResults.Ok(respuestaAutenticacion);
			}
			else
			{
                return TypedResults.BadRequest("Login incorrecto");
            }
		}

		private static RespuestaAutenticacionDTO ConstruirToken(CredencialesUsuarioDTO credencialesUsuarioDTO,
																IConfiguration configuration)
		{
			var claims = new List<Claim>
			{ 
				new Claim("email", credencialesUsuarioDTO.Email),
				new Claim("lo que yo quiera", "cualquier otro valor")
			};

			var llave = Llaves.ObtenerLlave(configuration);
			var creds = new SigningCredentials(llave.First(), SecurityAlgorithms.HmacSha256);

			var expiracion = DateTime.UtcNow.AddYears(1);

			var tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

			var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

			return new RespuestaAutenticacionDTO
			{
				Token = token,
				Expiracion = expiracion,
			};
		}
	}
}
