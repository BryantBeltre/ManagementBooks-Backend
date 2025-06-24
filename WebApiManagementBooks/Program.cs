using WebApiManagementBooks.Application.MappingProfiles;
using WebApiManagementBooks.Application.Services;
using WebApiManagementBooks.Application.Services.Interface;
using WebApiManagementBooks.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAutoMapper(typeof(BookProfile).Assembly);
builder.Services.AddAutoMapper(typeof(AuthorProfile).Assembly);

// Añadir servicios de la capa de Aplicación
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AthorService>();

// Añadir servicios de la capa de Infraestructura (repositorios y HttpClients)
builder.Services.AddInfrastructureServices();




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        .WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8080")
        .AllowAnyHeader()
        .AllowAnyMethod()
        );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
