using Microsoft.EntityFrameworkCore;
using DentalClinicAPI.Data;
using DentalClinicAPI.Repositories;
using DentalClinicAPI.Services;
using DentalClinicAPI.Mappings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddScoped<IAuthService, AuthService>();

// 5. Configura��o do JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 6. Configura��o de controllers com Newtonsoft.Json
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// 7. Configura��o do Swagger com documenta��o XML
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Dental Clinic API", Version = "v1" });
    c.EnableAnnotations(); // Para suporte a XML comments

    // Configura��o do arquivo XML de documenta��o
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Adicionar configura��o para incluir o token JWT no Swagger UI
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header usando o esquema Bearer. \r\n\r\n Digite 'Bearer' [espa�o] e seu token.\r\n\r\nExemplo: \"Bearer 12345abcdef\""
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// 8. Configura pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dental Clinic API v1");
        c.RoutePrefix = "swagger"; // URL: /swagger
    });
}

// 9. Middleware padr�o
app.UseHttpsRedirection();

// 10. Middleware de autentica��o (adicione antes do UseAuthorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 11. Cria o banco de dados se n�o existir (apenas para desenvolvimento)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClinicContext>();
    dbContext.Database.EnsureCreated(); // Substitua por migra��es em produ��o
}

app.Run();