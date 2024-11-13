namespace MininalAPIPeliculas.DTOs
{
    public class PeliculaDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public string? Poster { get; set; }
        public List<ComentarioDTO> comentarios { get; set; } = [];
        public List<GeneroDTO> generos { get; set; } = [];
        public List<ActorPeliculaDTO> actores { get; set; } = [];
    }
}
