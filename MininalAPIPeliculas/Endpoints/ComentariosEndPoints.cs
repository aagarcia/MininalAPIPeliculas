using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Filtros;
using MininalAPIPeliculas.Repositorios;

namespace MininalAPIPeliculas.Endpoints
{
    public static class ComentariosEndPoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/{peliculaId:int}/comentarios", ObtenerTodos)
                .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60))
                .Tag("comentarios-get")); ;
            group.MapGet("/comentarios/{id:int}", ObtenerPorId);
            group.MapPost("/{peliculaId:int}/comentarios", Crear).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>();
            group.MapPut("/{peliculaId:int}/comentarios/{id:int}", Actualizar).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>();
            group.MapDelete("/{peliculaId:int}/comentarios/{id:int}", Borrar);

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

        static async Task<Results<Created<ComentarioDTO>, NotFound>> Crear(int peliculaId,
                                                                           CrearComentarioDTO crearComentarioDTO,
                                                                           IRepositorioComentarios repositorioComentarios,
                                                                           IRepositorioPeliculas repositorioPeliculas,
                                                                           IMapper mapper,
                                                                           IOutputCacheStore outputCacheStore)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;
            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return TypedResults.Created($"/pelicula/comentarios/{id}", comentarioDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int peliculaId,
                                                                   int id,
                                                                   CrearComentarioDTO crearComentarioDTO,
                                                                   IRepositorioComentarios repositorio,
                                                                   IMapper mapper,
                                                                   IOutputCacheStore outputCacheStore)
        {
            if (!await repositorio.ExistePorPeliculaYId(id, peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.Id = id;
            comentario.PeliculaId = peliculaId;

            await repositorio.Actualizar(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int peliculaId,
                                                               int id,
                                                               IRepositorioComentarios repositorio,
                                                               IOutputCacheStore outputCacheStore)
        {
            if (!await repositorio.ExistePorPeliculaYId(id, peliculaId))
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }
    }
}
