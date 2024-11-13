using MininalAPIPeliculas.Entidades;

namespace MininalAPIPeliculas.Repositorios
{
    public interface IRepositorioComentarios
    {
        Task Actualizar(Comentario comentario);
        Task Borrar(int id);
        Task<int> Crear(Comentario comentario);
        Task<bool> Existe(int id);
        Task<bool> ExistePorPeliculaYId(int id, int peliculaId);
        Task<bool> ExistePorPelicula(int peliculaId);
        Task<Comentario?> ObtenerPorId(int id);
        Task<List<Comentario>> ObtenerTodos(int peliculaId);
    }
}