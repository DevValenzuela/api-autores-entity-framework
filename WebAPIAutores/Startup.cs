using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WebAPIAutores.Filtros;
using WebAPIAutores.Middlewares;
using WebAPIAutores.Servicios;
using WebAPIAutores.Utilidades;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace WebAPIAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
                opciones.Conventions.Add(new SwaggerAddPorVersion());
            }).AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opciones => opciones.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["key_jwt"])
                        ),
                        ClockSkew = TimeSpan.Zero
                    });

            //services.AddTransient<ServicioA>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebAPIAutores",
                    Version = "v1",
                    Description = "Este es un web api v1.",
                    Contact = new OpenApiContact
                    {
                        Email =  "vlzdavid@outlook.com",
                        Name = "David Valenzuela",
                        Url = new Uri("https://github.com/valenzuela21")
                    },
                    License = new OpenApiLicense
                    {
                        Name =  "MIT"
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebAPIAutores", Version = "v2", Description = "Este es un web api v2." });
                c.OperationFilter<AddParametreHATEOAS>();
                c.OperationFilter<AddParametroXVersion>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                     new OpenApiSecurityScheme
                     {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },

                     },
                     new string[] { }
                    }
                });

                var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
                c.IncludeXmlComments(rutaXML);

            });

            services.AddAutoMapper(typeof(Startup));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("isAdmin", polit => polit.RequireClaim("isAdmin"));
                opciones.AddPolicy("isReseller", polit => polit.RequireClaim("isReseller"));
            });

            services.AddDataProtection();
            services.AddTransient<HashService>();

            services.AddCors(opcions =>
            {
                opcions.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "cantidadTotalRegister" });
                });

            });

            services.AddTransient<GeneradorLinks>();
            services.AddTransient<HATEOASAutorFilterAttribute>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:connectionString"]);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            //app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            app.UseLoguearRespuestaHTTP();

            app.Map("/ruta1", app =>
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Estoy interceptando la tubería");
                });
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebAPIAutores v2");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
