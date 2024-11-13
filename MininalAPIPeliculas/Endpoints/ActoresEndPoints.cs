using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Filtros;
using MininalAPIPeliculas.Repositorios;
using MininalAPIPeliculas.Servicios;

namespace MininalAPIPeliculas.Endpoints
{
    public static class ActoresEndPoints
    {
        private static readonly string contenedor = "actores";
        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerActores);
                //.CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60))
                //.Tag("actores-get"));

            group.MapGet("/{id:int}", ObtenerPorId);

            group.MapGet("/obtenerPorNombre/{nombre}", ObtenerPorNombre);

            group.MapPost("/", CrearActor).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearActorDTO>>();

            group.MapPut("/{id:int}", ActualizarActor).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearActorDTO>>();

            group.MapDelete("/{id:int}", BorrarActor);

            return group;
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerActores(IRepositorioActores repositorio,
                                                             IMapper mapper,
                                                             int pagina = 1,
                                                             int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina
            };
            var actores = await repositorio.ObtenerTodos(paginacion);
            var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDTO);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerPorId(IRepositorioActores repositorio,
                                                                        int id,
                                                                        IMapper mapper)
        {
            var actor = await repositorio.ObtenerPorId(id);

            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            var actorDTO = mapper.Map<ActorDTO>(actor);

            return TypedResults.Ok(actorDTO);
        }

        static async Task<Ok<List<ActorDTO>>> ObtenerPorNombre(string nombre,
                                                               IRepositorioActores repositorio,
                                                               IMapper mapper)
        {
            var actores = await repositorio.ObtenerPorNombre(nombre);
            var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
            return TypedResults.Ok(actoresDTO);
        }

        static async Task<Results<Created<ActorDTO>, ValidationProblem>> CrearActor([FromForm] CrearActorDTO crearActorDTO,
                                                        IRepositorioActores repositorio,
                                                        IOutputCacheStore outputCacheStore,
                                                        IMapper mapper,
                                                        IAlmacenadorArchivos almacenadorArchivos)
        {
            var actor = mapper.Map<Actor>(crearActorDTO);

            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearActorDTO.Foto);
                actor.Foto = url;
            }

            var id = await repositorio.Crear(actor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actores/{id}", actorDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarActor(int id,
                                                                        [FromForm] CrearActorDTO crearActorDTO,
                                                                        IRepositorioActores repositorio,
                                                                        IOutputCacheStore outputCacheStore,
                                                                        IAlmacenadorArchivos almacenadorArchivos,
                                                                        IMapper mapper)
        {
            var actorDB = await repositorio.ObtenerPorId(id);

            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }

            var actor = mapper.Map<Actor>(crearActorDTO);
            actor.Id = id;
            actor.Foto = actorDB.Foto;

            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Editar(actor.Foto, contenedor, crearActorDTO.Foto);
                actor.Foto = url;
            }


            await repositorio.Actualizar(actor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> BorrarActor(int id,
                                                                    IRepositorioActores repositorio,
                                                                    IOutputCacheStore outputCacheStore,
                                                                    IAlmacenadorArchivos almacenadorArchivos)
        {
            var actorDB = await repositorio.ObtenerPorId(id);

            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(actorDB.Foto, contenedor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }
    }
}
