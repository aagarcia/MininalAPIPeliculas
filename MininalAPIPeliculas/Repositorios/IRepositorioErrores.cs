using MininalAPIPeliculas.Entidades;

namespace MininalAPIPeliculas.Repositorios
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}