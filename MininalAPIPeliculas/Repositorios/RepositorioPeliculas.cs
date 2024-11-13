using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MininalAPIPeliculas.DTOs;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Utilidades;

namespace MininalAPIPeliculas.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(ApplicationDbContext context,
                                    IHttpContextAccessor httpContextAccessor,
                                    IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas
                                   .Include(p => p.Comentarios)
                                   .Include(p => p.GenerosPeliculas)
                                    .ThenInclude(gp => gp.Genero)
                                   .Include(p => p.ActoresPeliculas.OrderBy(a => a.Orden))
                                    .ThenInclude(ap => ap.Actor)
                                   .AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(p => p.Titulo).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await context.Peliculas
                                .Include(p => p.Comentarios)
                                .Include(p => p.GenerosPeliculas)
                                    .ThenInclude(gp => gp.Genero)
                                .Include(p => p.ActoresPeliculas.OrderBy(a => a.Orden))
                                    .ThenInclude(ap => ap.Actor)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(p => p.Id == id);
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
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Peliculas.Where(p => p.Id == id).ExecuteDeleteAsync();
        }

        public async Task AsignarGeneros(int id, List<int> generosId)
        {
            var pelicula = await context.Peliculas
                                        .Include(p => p.GenerosPeliculas)
                                        .FirstOrDefaultAsync(p => p.Id == id) ?? 
                           throw new ArgumentException($"No existe una película con el id: {id}");

            var generosPeliculas = generosId.Select(generoId => new GeneroPelicula() { GeneroId = generoId});

            pelicula.GenerosPeliculas = mapper.Map(generosPeliculas, pelicula.GenerosPeliculas);

            await context.SaveChangesAsync();
        }

        public async Task AsignarActores(int id, List<ActorPelicula> actores)
        {
            for (int i = 1; i <= actores.Count; i++)
            {
                actores[i-1].Orden = i;
            }

            var pelicula = await context.Peliculas
                                        .Include(x => x.ActoresPeliculas)
                                        .FirstOrDefaultAsync(p => p.Id == id) ??
                           throw new ArgumentException($"No existe una película con el id: {id}");

            pelicula.ActoresPeliculas = mapper.Map(actores, pelicula.ActoresPeliculas);

            await context.SaveChangesAsync();
        }
    }
}
