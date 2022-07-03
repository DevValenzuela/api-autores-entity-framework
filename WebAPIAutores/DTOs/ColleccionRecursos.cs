namespace WebAPIAutores.DTOs
{
    public class ColleccionRecursos<T> : Recurso where T : Recurso
    {
        public List<T> Valores { get; set; }

    }
}
