using Microsoft.EntityFrameworkCore;
using DentalClinicAPI.Data;
using DentalClinicAPI.Repositories;
using DentalClinicAPI.Services;
using DentalClinicAPI.Mappings; // Adicione para o AutoMapperProfile
using AutoMapper; // Adicione para suporte ao AutoMapper
using Microsoft.AspNetCore.Mvc.NewtonsoftJson; // Adicione para Newtonsoft.Json
using System.Reflection; // Adicione para obter informações do assembly

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do Entity Framework Core com Oracle
builder.Services.AddDbContext<ClinicContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// 2. Configuração do AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// 3. Registra repositórios genéricos
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// 4. Registra serviços
builder.Services.AddScoped<IClinicService, ClinicService>();

// 5. Configuração de controllers com Newtonsoft.Json
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// 6. Configuração do Swagger com documentação XML
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Dental Clinic API", Version = "v1" });
    c.EnableAnnotations(); // Para suporte a XML comments

    // Configuração do arquivo XML de documentação
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

// 8. Middleware padrão
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 9. Cria o banco de dados se não existir (apenas para desenvolvimento)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClinicContext>();
    dbContext.Database.EnsureCreated(); // Substitua por migrações em produção
}

app.Run();