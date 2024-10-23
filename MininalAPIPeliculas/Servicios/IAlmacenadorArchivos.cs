namespace MininalAPIPeliculas.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task Borrar(String? ruta, string contenedor);
        Task<string> Almacenar(string contenedor, IFormFile archivo);
        async Task<string> Editar(string? ruta, string contenedor, IFormFile archivo)
        {
            await Borrar(ruta, contenedor);
            return await Almacenar(contenedor, archivo);
        }
    }
}
