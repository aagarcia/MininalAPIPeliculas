namespace MininalAPIPeliculas.Entidades
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string? Poster { get; set; }

        public List<Comentario> Comentarios { get; set; } = [];
        public List<GeneroPelicula> GenerosPeliculas { get; set; } = [];
        public List<ActorPelicula> ActoresPeliculas { get; set; } = [];
    }
}
