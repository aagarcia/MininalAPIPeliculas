using AutoMapper;
using MininalAPIPeliculas.Repositorios;

namespace MininalAPIPeliculas.DTOs
{
    public class ObtenerGeneroPorIdPeticionDTO
    {
        public IRepositorioGeneros repositorio { get; set; }
        public int id { get; set; }
        public IMapper mapper { get; set; }
    }
}
