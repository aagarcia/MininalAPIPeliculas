using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Repositorios;
using MininalAPIPeliculas.Servicios;

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

            group.MapPost("/", CrearPelicula).DisableAntiforgery();

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
    }
}
