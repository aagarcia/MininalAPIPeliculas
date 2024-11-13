using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;

namespace MininalAPIPeliculas.Repositorios
{
    public interface IRepositorioActores
    {
        Task<List<Actor>> ObtenerTodos(PaginacionDTO paginacionDTO);
        Task<Actor?> ObtenerPorId(int id);
        Task<List<Actor>> ObtenerPorNombre(string nombre);
        Task<bool> Existe(int id);
        Task<List<int>> Existen(List<int> ids);
        Task<int> Crear(Actor actor);
        Task Actualizar(Actor actor);
        Task Borrar(int id);
    }
}
