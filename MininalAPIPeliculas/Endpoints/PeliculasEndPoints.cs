﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Filtros;
using MininalAPIPeliculas.Repositorios;
using MininalAPIPeliculas.Servicios;
using MininalAPIPeliculas.Validaciones;

namespace MininalAPIPeliculas.Endpoints
{
    public static class PeliculasEndPoints
    {
        private static readonly string contenedor = "peliculas";

        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerPeliculas)
                .CacheOutput(x => x.Expire(TimeSpan.FromSeconds(60))
                .Tag("peliculas-get"));

            group.MapGet("/{id:int}", ObtenerPorId);

            group.MapPost("/", CrearPelicula)
                 .DisableAntiforgery()
                 .AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>()
                 .RequireAuthorization("esadmin");

            group.MapPut("/{id:int}", ActualizarPelicula)
                 .DisableAntiforgery()
                 .AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>()
                 .RequireAuthorization("esadmin");

            group.MapDelete("/{id:int}", BorrarPelicula)
                 .RequireAuthorization("esadmin");

            group.MapPost("/{id:int}/asignargeneros", AsignarGeneros)
                 .RequireAuthorization("esadmin");

            group.MapPost("/{id:int}/asignaractores", AsignarActores)
                 .RequireAuthorization("esadmin");

            return group;
        }

        static async Task<Ok<List<PeliculaDTO>>> ObtenerPeliculas(IRepositorioPeliculas repositorio,
                                                                  IMapper mapper,
                                                                  int pagina = 1,
                                                                  int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO 
            {
                Pagina = pagina,
                RecordsPorPagina = recordsPorPagina,
            };
            var peliculas = await repositorio.ObtenerTodos(paginacion);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
            return TypedResults.Ok(peliculasDTO);
        }

        static async Task<Results<Ok<PeliculaDTO>, NotFound>> ObtenerPorId(IRepositorioPeliculas repositorio,
                                                                           IMapper mapper,
                                                                           int id)
        {
            var pelicula = await repositorio.ObtenerPorId(id);

            if (pelicula is null)
            {
                TypedResults.NotFound();
            }

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Ok(peliculaDTO);
        }

        static async Task<Created<PeliculaDTO>> CrearPelicula([FromForm] CrearPeliculaDTO crearPeliculaDTO,
                                                           IRepositorioPeliculas repositorio,
                                                           IOutputCacheStore outputCacheStore,
                                                           IMapper mapper,
                                                           IAlmacenadorArchivos almacenadorArchivos)
        {
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            var id = await repositorio.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Created($"/peliculas/{id}", peliculaDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarPelicula(int id,
                                                                           [FromForm] CrearPeliculaDTO crearPeliculaDTO,
                                                                           IRepositorioPeliculas repositorio,
                                                                           IOutputCacheStore outputCacheStore,
                                                                           IMapper mapper,
                                                                           IAlmacenadorArchivos almacenadorArchivos)
        {
            var peliculaDB = await repositorio.ObtenerPorId(id);

            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);
            pelicula.Id = id;
            pelicula.Poster = peliculaDB.Poster;

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Editar(pelicula.Poster, 
                                                           contenedor,
                                                           crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            await repositorio.Actualizar(pelicula);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> BorrarPelicula(int id,
                                                                       IRepositorioPeliculas repositorio,
                                                                       IOutputCacheStore outputCacheStore,
                                                                       IAlmacenadorArchivos almacenadorArchivos)
        {
            var peliculaDB = await repositorio.ObtenerPorId(id);

            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(peliculaDB.Poster, contenedor);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id,
                                                                                           List<int> generosIds,
                                                                                           IRepositorioPeliculas repositorioPeliculas,
                                                                                           IRepositorioGeneros repositorioGeneros)
        {
            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var generosExistentes = new List<int>();

            if (generosIds.Count != 0)
            {
                generosExistentes = await repositorioGeneros.Existen(generosIds);
            }

            if (generosExistentes.Count != generosIds.Count)
            {
                var generosNoExistentes = generosIds.Except(generosExistentes);

                return TypedResults.BadRequest($"Los generos de id {string.Join(",", generosNoExistentes)} no existen.");
            }

            await repositorioPeliculas.AsignarGeneros(id, generosIds);

            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignarActores(int id,
                                                                                           List<AsignarActorPeliculaDTO> actoresDTO,
                                                                                           IRepositorioPeliculas repositorioPeliculas,
                                                                                           IRepositorioActores repositorioActores,
                                                                                           IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(a => a.ActorId).ToList();

            if (actoresDTO.Count != 0)
            {
                actoresExistentes = await repositorioActores.Existen(actoresIds);
            }

            if (actoresExistentes.Count != actoresDTO.Count)
            {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);
                return TypedResults.BadRequest($"Los actores de id {string.Join(",", actoresNoExistentes)} no existen.");
            }

            var actores = mapper.Map<List<ActorPelicula>>(actoresDTO);

            await repositorioPeliculas.AsignarActores(id, actores);

            return TypedResults.NoContent();
        }
    }
}
