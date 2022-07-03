using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService =  authorizationService;
        }

        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();
            var isAdmin = await authorizationService.AuthorizeAsync(User, "isAdmin");
            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerRoot", new { }), description: "self", metodo: "GET"));
            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), description: "autores", metodo: "GET"));

            if (isAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearAutor", new { }), description: "autor-crear", metodo: "POST"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("crearLibro", new { }), description: "libro-crear", metodo: "POST"));

            }

            return datosHateoas;
        }
    }
}
