using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Servicios
{
    public class GeneradorLinks
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorLinks(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor =  actionContextAccessor;
        }

        private async Task<bool> esAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var adminValidate = await authorizationService.AuthorizeAsync(httpContext.User, "isAdmin");
            return adminValidate.Succeeded;

        }

        private IUrlHelper ConstruirURLHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        public async Task GenerarLink(AutorDTO autorDTO)
        {
            var isAdmin = await esAdmin();
            var Url = ConstruirURLHelper();

            autorDTO.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerAutor", new { id = autorDTO.Id }),
                description: "self",
                metodo: "GET"));

            if (isAdmin)
            {
                autorDTO.Enlaces.Add(new DatoHATEOAS(
           enlace: Url.Link("actualizarAutor", new { id = autorDTO.Id }),
           description: "self",
           metodo: "PUT"));

                autorDTO.Enlaces.Add(new DatoHATEOAS(
                  enlace: Url.Link("eliminarAutor", new { id = autorDTO.Id }),
                  description: "self",
                  metodo: "DELETE"));
            }


        }
    }
}
