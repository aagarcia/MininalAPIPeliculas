using Microsoft.EntityFrameworkCore;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Utilidades;

namespace MininalAPIPeliculas.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext HttpContext;

        public RepositorioPeliculas(ApplicationDbContext context,
                                    IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            HttpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(p => p.Titulo).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await context.Peliculas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(a => a.Id == id);
        }

        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task Actualizar(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Peliculas.Where(p => p.Id == id).ExecuteDeleteAsync();
        }
    }
}
