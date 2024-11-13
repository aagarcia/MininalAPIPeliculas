using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;

namespace MininalAPIPeliculas.Repositorios
{
    public interface IRepositorioPeliculas
    {
        Task Actualizar(Pelicula pelicula);
        Task Borrar(int id);
        Task<int> Crear(Pelicula pelicula);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPorId(int id);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO);
        Task AsignarGeneros(int id, List<int> generosId);
        Task AsignarActores(int id, List<ActorPelicula> actores);
    }
}