using System.ComponentModel.DataAnnotations;

namespace MininalAPIPeliculas.Entidades
{
    public class Genero
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;
    }
}
