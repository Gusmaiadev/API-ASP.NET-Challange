using Microsoft.EntityFrameworkCore;
using DentalClinicAPI.Data;
using DentalClinicAPI.Repositories;
using DentalClinicAPI.Services;
using DentalClinicAPI.Mappings; // Adicione para o AutoMapperProfile
using AutoMapper; // Adicione para suporte ao AutoMapper
using Microsoft.AspNetCore.Mvc.NewtonsoftJson; // Adicione para Newtonsoft.Json
using System.Reflection; // Adicione para obter informa��es do assembly

var builder = WebApplication.CreateBuilder(args);

// 1. Configura��o do Entity Framework Core com Oracle
builder.Services.AddDbContext<ClinicContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// 2. Configura��o do AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// 3. Registra reposit�rios gen�ricos
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 4. Registra servi�os
builder.Services.AddScoped<IClinicService, ClinicService>();

// 5. Configura��o de controllers com Newtonsoft.Json
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// 6. Configura��o do Swagger com documenta��o XML
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Dental Clinic API", Version = "v1" });
    c.EnableAnnotations(); // Para suporte a XML comments

    // Configura��o do arquivo XML de documenta��o
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// 7. Configura pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dental Clinic API v1");
        c.RoutePrefix = "swagger"; // URL: /swagger
    });
}

// 8. Middleware padr�o
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 9. Cria o banco de dados se n�o existir (apenas para desenvolvimento)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClinicContext>();
    dbContext.Database.EnsureCreated(); // Substitua por migra��es em produ��o
}

app.Run();