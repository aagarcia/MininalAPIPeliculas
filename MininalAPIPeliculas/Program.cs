using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MininalAPIPeliculas;
using MininalAPIPeliculas.Endpoints;
using MininalAPIPeliculas.Entidades;
using MininalAPIPeliculas.Migrations;
using MininalAPIPeliculas.Repositorios;
using MininalAPIPeliculas.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Inicio de area de los servicios

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

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

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

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

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", () => "Hello World!");

app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();

// Fin de area de los middleware

app.Run();
