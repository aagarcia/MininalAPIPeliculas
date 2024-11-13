using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MininalAPIPeliculas;
using MininalAPIPeliculas.Endpoints;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Repositorios;
using MininalAPIPeliculas.Servicios;
using MininalAPIPeliculas.Utilidades;

var builder = WebApplication.CreateBuilder(args);

// Inicio de area de los servicios

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddIdentityCore<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();

builder.Services.AddCors(opciones => 
    opciones.AddDefaultPolicy(configuraciones => {
        configuraciones.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    }));

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer(opciones => new TokenValidationParameters
{
    ValidateIssuer = false, //si no queremos validar el emisor
    ValidateAudience = false, //es para quien esta destinado el token
    ValidateLifetime = true, //tiempo de vida del token
    ValidateIssuerSigningKey = true, //para validar si el token esta debidamente firmado por una llave
    //podemos utilizar multiples llaves o una llave especifica
    //IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First(), //esto es para una llave
    IssuerSigningKeys = Llaves.ObtenerTodasLasLlave(builder.Configuration), // para obtener todas las llaves
    ClockSkew = TimeSpan.Zero, //para no tener problemas de diferencias de tiempo al evaluar la llave si vencio o no
});
builder.Services.AddAuthorization();

// Fin de area de los servicios

var app = builder.Build();

// Inicio de area de los middleware

//si deseo que swagger no se ejecute en development
//if (builder.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandleFeature = context.Features.Get<IExceptionHandlerFeature>();
    var excepcion = exceptionHandleFeature?.Error!;

    var error = new Error
    {
        Fecha = DateTime.UtcNow,
        MensajeDeError = excepcion.Message,
        StackTrace = excepcion.StackTrace
    };

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>();
    await repositorio.Crear(error);

    await TypedResults.BadRequest(new { 
        tipo = "error",
        mensaje = "ha ocurrido un mensaje de error inesperado",
        estatus = 500
    }).ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapGet("/error", () => {
    throw new InvalidOperationException("Error de ejemplo");
});

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula").MapComentarios();
app.MapGroup("/usuarios").MapUsuarios();

// Fin de area de los middleware

app.Run();
