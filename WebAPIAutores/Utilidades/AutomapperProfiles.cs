using AutoMapper;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;

namespace WebAPIAutores.Utilidades
{
    public class AutomapperProfiles : Profile
    {
        public AutomapperProfiles()
        {
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorDTO>();
            CreateMap<Autor, AutorDtoConLibros>().
                 ForMember(autorDtoConLibros => autorDtoConLibros.Libros, opciones => opciones.MapFrom(MapAutoresLibrosDTO));
            CreateMap<LibroCreacionDTO, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>().ReverseMap();
            CreateMap<Libro, LibroDTOConAutores>()
                .ForMember(libroDTOConAutores => libroDTOConAutores.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));
            CreateMap<LibroPathDTO, Libro>().ReverseMap();
            CreateMap<ComentarioCreacionDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();


        }
        private List<LibroDTO> MapAutoresLibrosDTO(Autor autor, AutorDTO autorDTO)
        {
            var result = new List<LibroDTO>();

            if (autor.AutoresLibros == null) { return result; }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                result.Add(new LibroDTO
                {

                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }
            return result;
        }


        private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var result = new List<AutorDTO>();
            if (libro.AutoresLibros == null) { return result; }
            foreach (var autorLibro in libro.AutoresLibros)
            {
                result.Add(new AutorDTO()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return result;
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if (libroCreacionDTO.AutoresId == null) { return resultado; }

            foreach (var autorId in libroCreacionDTO.AutoresId)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }

            return resultado;
        }
    }
}
