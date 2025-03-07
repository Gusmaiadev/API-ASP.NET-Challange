using Microsoft.EntityFrameworkCore;
using DentalClinicAPI.Data;
using DentalClinicAPI.Repositories;
using DentalClinicAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework Core com Oracle
builder.Services.AddDbContext<ClinicContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// Registra repositórios genéricos
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Registra serviços (Singleton)
builder.Services.AddScoped<IClinicService, ClinicService>();

// Configuração padrão da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Dental Clinic API", Version = "v1" });
});

var app = builder.Build();

// Configura pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dental Clinic API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();