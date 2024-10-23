using AutoMapper;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;

namespace MininalAPIPeliculas.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<Genero, GeneroDTO>();

            CreateMap<CrearActorDTO, Actor>()
                .ForMember(x => x.Foto, opciones => opciones.Ignore());
            CreateMap<Actor, ActorDTO>();

            CreateMap<CrearPeliculaDTO, Pelicula>()
                .ForMember(x => x.Poster, opciones => opciones.Ignore());
            CreateMap<Pelicula, PeliculaDTO>();
        }
    }
}
