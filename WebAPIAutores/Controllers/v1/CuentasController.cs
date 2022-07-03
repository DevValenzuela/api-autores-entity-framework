using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPIAutores.DTOs;
using WebAPIAutores.Servicios;

namespace WebAPIAutores.Controllers.V1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService
            )
        {
            this.userManager =  userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico_quizas_secreto");
        }
        [HttpGet("hash/{textPlano}")]
        public ActionResult RealizarHash(string textPlano)
        {
            var result1 = hashService.Hash(textPlano);
            var result2 = hashService.Hash(textPlano);

            return Ok(new
            {
                textPlano = textPlano,
                result1 = result1,
                result2 = result2
            });
        }


        [HttpGet("encriptar")]
        public ActionResult Encriptar()
        {
            var textPlano = "Felipe Gavilán";
            var textCifrado = dataProtector.Protect(textPlano);
            var textDesencriptado = dataProtector.Unprotect(textCifrado);

            return Ok(new
            {
                textoPlano = textPlano,
                textoCifrado = textCifrado,
                textDesencriptado = textDesencriptado

            });
        }

        [HttpGet("encriptarTime")]
        public ActionResult EncriptarTime()
        {
            var timeLimitPortector = dataProtector.ToTimeLimitedDataProtector();
            try
            {
                var textPlano = "Felipe Gavilán";
                var textCifrado = timeLimitPortector.Protect(textPlano, lifetime: TimeSpan.FromSeconds(5));
                Thread.Sleep(6000);
                var textDesencriptado = timeLimitPortector.Unprotect(textCifrado);
                return Ok(new
                {
                    textoPlano = textPlano,
                    textoCifrado = textCifrado,
                    textDesencriptado = textDesencriptado

                });
            }
            catch (Exception ex)
            {
                throw new Exception("La encriptación vencio.");
            }

        }

        [HttpPost("login", Name = "ingresarUsuario")]
        public async Task<ActionResult<RespuestaAuthDTO>> login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var result = await signInManager.PasswordSignInAsync(credencialesUsuarioDTO.Email,
                credencialesUsuarioDTO.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var resp = await ConstructorToken(credencialesUsuarioDTO);
                return resp;
            }
            else
            {
                return BadRequest("Login Incorrecto");
            }
        }

        [HttpPost("registrar", Name = "registrarUsuario")]
        public async Task<ActionResult<RespuestaAuthDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var usuario = new IdentityUser
            {
                UserName = credencialesUsuarioDTO.Email,
                Email = credencialesUsuarioDTO.Email
            };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

            if (resultado.Succeeded)
            {
                var resp = await ConstructorToken(credencialesUsuarioDTO);
                return resp;
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpGet("RenovarToken", Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAuthDTO>> renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var credentialsUsers = new CredencialesUsuarioDTO()
            {
                Email = email,

            };

            var resp = await ConstructorToken(credentialsUsers);
            return resp;
        }
        private async Task<RespuestaAuthDTO> ConstructorToken(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var claims = new List<Claim>() {
                new Claim("email", credencialesUsuarioDTO.Email),
                new Claim("user", credencialesUsuarioDTO.Email)
            };

            var user = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["key_jwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(30);
            var securityToken = new JwtSecurityToken(
                issuer: null, audience: null, claims: claims, expires: expiration, signingCredentials: creds);

            return new RespuestaAuthDTO()
            {
                Token =  new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiration
            };
        }

        [HttpPost("HacerAdmin", Name = "hacerAdmin")]
        public async Task<ActionResult> hacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.AddClaimAsync(user, new Claim("isAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoveAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> removeAdmin(EditarAdminDTO editarAdminDTO)
        {
            var user = await userManager.FindByEmailAsync(editarAdminDTO.Email);
            await userManager.RemoveClaimAsync(user, new Claim("isAdmin", "1"));
            return NoContent();
        }
    }

}

