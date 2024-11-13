using MininalAPIPeliculas.Entidades;

namespace MininalAPIPeliculas.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task<List<Genero>> ObtenerTodos();
        Task<Genero?> ObtenerPorId(int id);
        Task<bool> Existe(int id);
		Task<bool> Existe(int id, string nombre);
        Task<List<int>> Existen(List<int> ids);
        Task<int> Crear(Genero genero);
        Task Actualizar(Genero genero);
        Task Borrar(int id);
	}
}
