using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Validaciones;

namespace WebAPIAutores.DTOs
{
    public class LibroCreacionDTO
    {
        [PrimeraLetraMayuscula]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no debe de tener más de {1} carácteres")]
        [Required]
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public List<int> AutoresId { get; set; }
    }
}
