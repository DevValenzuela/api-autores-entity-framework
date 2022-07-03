using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Entidades;
using WebAPIAutores.Utilidades;

namespace WebAPIAutores.Controllers.V2
{
    [ApiController]
    [Route("api/autores")]
    [CabeceraEstarPresente("x-version", "2")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isAdmin")]
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


        [HttpGet(Name = "obtenerAutoresv2")] // api/autores
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            var autors = await context.Autores.ToListAsync();
            autors.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
            return mapper.Map<List<AutorDTO>>(autors);
        }

        [HttpGet("{id:int}", Name = "obtenerAutov2r")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HATEOASAutorFilterAttribute))]
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


        [HttpGet("{nombre}", Name = "obtenerPorNombrev2")] // api/autores
        public async Task<ActionResult<List<AutorDTO>>> GetPorNombre([FromRoute] string nombre)
        {
            var autor = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();

            if (autor == null)
            {
                return NotFound();
            }
            return mapper.Map<List<AutorDTO>>(autor);
        }


        [HttpGet("primero", Name = "primerAutorporNombreyAppellidov2")] // api/autores/primero?nombre=felipe&apellido=gavilan
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor, [FromQuery] string nombre)
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        [HttpGet("primero2")] // api/autores/primero
        public ActionResult<Autor> PrimerAutor2()
        {
            return new Autor() { Nombre = "Inventadov2" };
        }


        [HttpPost(Name = "crearAutorv2")]
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
            return CreatedAtRoute("obtenerAutorv2", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "actualizarAutorv2")] // api/autores/1 
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

        [HttpDelete("{id:int}", Name = "eliminarAutorv2")] // api/autores/2
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
