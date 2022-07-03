using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIAutores.DTOs;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Utilidades
{
    public class HATEOASAutorFilterAttribute : HATEOASFiltroAttribute
    {
        private readonly GeneradorLinks generadorLinks;

        public HATEOASAutorFilterAttribute(GeneradorLinks generadorLinks)
        {
            this.generadorLinks = generadorLinks;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            var include = DebeIncluirHATEOAS(context);

            if (!include)
            {
                await next();
                return;
            }

            var result = context.Result as ObjectResult;

            var autorDTO = result.Value as AutorDTO;
            if (autorDTO ==  null)
            {
                var autoresDTO = result.Value as List<AutorDTO> ??
                    throw new ArgumentException("Se esperaba una instancia de AutorDTO o List<AutorDTO>");

                autoresDTO.ForEach(async autorDTO => await generadorLinks.GenerarLink(autorDTO));
                result.Value = autoresDTO;
            }
            else
            {
                await generadorLinks.GenerarLink(autorDTO);
            }

            await next();

        }
    }
}
