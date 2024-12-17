using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Filtros;
using MininalAPIPeliculas.Repositorios;

namespace MininalAPIPeliculas.Endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros)
                .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60))
                .Tag("generos-get"));

            group.MapGet("/{id:int}", ObtenerGeneroPorId)
                 .AddEndpointFilter<FiltroDePrueba>();

            group.MapPost("/", CrearGenero)
                 .AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>()
                 .RequireAuthorization("esadmin");

            group.MapPut("/{id:int}", ActualizarGenero)
                 .AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>()
                 .RequireAuthorization("esadmin");

            group.MapDelete("/{id:int}", BorrarGenero)
                 .RequireAuthorization("esadmin");

            return group;
        }

        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio,
                                                              IMapper mapper,
                                                              ILoggerFactory loggerFactory)
        {
            var tipo = typeof(GenerosEndpoints);
            var logger = loggerFactory.CreateLogger(tipo.FullName!);
            //logger.LogInformation("Obteniendo el listado de generos");

            logger.LogTrace("Esto es un mensaje de trace");
            logger.LogDebug("Esto es un mensaje de debug");
            logger.LogInformation("Esto es un mensaje de information");
            logger.LogWarning("Esto es un mensaje de warning");
            logger.LogError("Esto es un mensaje de error");
            logger.LogCritical("Esto es un mensaje de critical");

            var generos = await repositorio.ObtenerTodos();

            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId([AsParameters] ObtenerGeneroPorIdPeticionDTO modelo)
        {
            var genero = await modelo.repositorio.ObtenerPorId(modelo.id);

            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            var generoDTO = modelo.mapper.Map<GeneroDTO>(genero);

            return TypedResults.Ok(generoDTO);
        }

        static async Task<Results<Created<GeneroDTO>, ValidationProblem>> CrearGenero(CrearGeneroDTO crearGeneroDTO,
                                                          IRepositorioGeneros repositorio,
                                                          IOutputCacheStore outputCacheStore,
                                                          IMapper mapper)
        {
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return TypedResults.Created($"/generos/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound, ValidationProblem>> ActualizarGenero(int id,
                                                                         CrearGeneroDTO crearGeneroDTO,
                                                                         IRepositorioGeneros repositorio,
                                                                         IOutputCacheStore outputCacheStore,
                                                                         IMapper mapper)
        {
			var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id;

            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> BorrarGenero(int id,
                                                                     IRepositorioGeneros repositorio,
                                                                     IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);

            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
    }
}
