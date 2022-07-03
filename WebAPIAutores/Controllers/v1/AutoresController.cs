using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;
using WebAPIAutores.Utilidades;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/autores")]
    [CabeceraEstarPresente("x-version", "1")]
    //[Route("api/v1/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(ApplicationDbContext context, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.context = context;
            this.mapper = mapper;
            this.authorizationService =  authorizationService;
        }


        [HttpGet(Name = "obtenerAutoresv1")] // api/autores
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.InsertParemetrePageHeader(queryable);
            var autors = await queryable.OrderBy(autor => autor.Nombre).Paginar(paginationDTO).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autors);
        }

        [HttpGet("{id:int}", Name = "obtenerAutorv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(200)]
        public async Task<ActionResult<AutorDtoConLibros>> Get(int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorDB => autorDB.Id == id);

            if (autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDtoConLibros>(autor);
            return dto;
        }


        [HttpGet("{nombre}", Name = "obtenerPorNombrev1")] // api/autores
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre)
        {
            var autor = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();

            if (autor == null)
            {
                return NotFound();
            }
            return mapper.Map<List<AutorDTO>>(autor);
        }


        [HttpGet("primero", Name = "primerAutorporNombreyAppellidov1")] // api/autores/primero?nombre=felipe&apellido=gavilan
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpGet("primero2")] // api/autores/primero
        public ActionResult<Autor> PrimerAutor2()
        {
            return new Autor() { Nombre = "Inventado" };
        }


        [HttpPost(Name = "crearAutor")]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutorConElMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);

            if (existeAutorConElMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();
            var autorDTO = mapper.Map<AutorDTO>(autor);
            return CreatedAtRoute("obtenerAutorv1", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutorv1")] // api/autores/1 
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
        {

            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;
            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Borrar un autor
        /// </summary>
        /// <param name="id">Id Buttom al borrar</param>
        /// <returns></returns>

        [HttpDelete("{id:int}", Name = "eliminarAutorv1")] // api/autores/2
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
