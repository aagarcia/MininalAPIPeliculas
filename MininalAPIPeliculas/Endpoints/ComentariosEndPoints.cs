using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Filtros;
using MininalAPIPeliculas.Repositorios;
using MininalAPIPeliculas.Servicios;

namespace MininalAPIPeliculas.Endpoints
{
    public static class ComentariosEndPoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/{peliculaId:int}/comentarios", ObtenerTodos)
                 .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60))
                 .Tag("comentarios-get"));
            group.MapGet("/comentarios/{id:int}", ObtenerPorId);
            group.MapPost("/{peliculaId:int}/comentarios", Crear)
                 .AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>();
            group.MapPut("/{peliculaId:int}/comentarios/{id:int}", Actualizar)
                 .AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>()
                 .RequireAuthorization();
            group.MapDelete("/{peliculaId:int}/comentarios/{id:int}", Borrar)
                 .RequireAuthorization();

            return group;
        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId,
                                                                                   IRepositorioComentarios repositorioComentarios,
                                                                                   IRepositorioPeliculas repositorioPeliculas,
                                                                                   IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            //if (!await repositorioComentarios.ExistePorPelicula(peliculaId))
            //{
            //    return TypedResults.NotFound();
            //} Con un poco mas de detalles en la respuesta seria mas adelante

            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentariosDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentariosDTO);
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int id,
                                                                             IRepositorioComentarios repositorio,
                                                                             IMapper mapper)
        {
            if (!await repositorio.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var comentario = await repositorio.ObtenerPorId(id);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<Created<ComentarioDTO>, 
                                  NotFound, 
                                  BadRequest<string>>> Crear(int peliculaId,
                                                             CrearComentarioDTO crearComentarioDTO,
                                                             IRepositorioComentarios repositorioComentarios,
                                                             IRepositorioPeliculas repositorioPeliculas,
                                                             IMapper mapper,
                                                             IOutputCacheStore outputCacheStore,
                                                             IServicioUsuarios servicioUsuarios)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.BadRequest("Usuario no encontrado");
            }

            comentario.UsuarioId = usuario.Id;

            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return TypedResults.Created($"/pelicula/comentarios/{id}", comentarioDTO);
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Actualizar(int peliculaId,
                                                                   int id,
                                                                   CrearComentarioDTO crearComentarioDTO,
                                                                   IRepositorioComentarios repositorio,
                                                                   IOutputCacheStore outputCacheStore,
                                                                   IServicioUsuarios servicioUsuarios)
        {
            if (!await repositorio.ExistePorPeliculaYId(id, peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarioBD = await repositorio.ObtenerPorId(id);

            if (comentarioBD is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (usuario.Id != comentarioBD.UsuarioId)
            {
                return TypedResults.Forbid();
            }

            comentarioBD.Cuerpo = crearComentarioDTO.Cuerpo;

            await repositorio.Actualizar(comentarioBD);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Borrar(int peliculaId,
                                                               int id,
                                                               IRepositorioComentarios repositorio,
                                                               IOutputCacheStore outputCacheStore,
                                                               IServicioUsuarios servicioUsuarios)
        {
            if (!await repositorio.ExistePorPeliculaYId(id, peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarioBD = await repositorio.ObtenerPorId(id);

            if (comentarioBD is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuarios.ObtenerUsuario();

            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (usuario.Id != comentarioBD.UsuarioId)
            {
                return TypedResults.Forbid();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }
    }
}
