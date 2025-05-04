using Microsoft.EntityFrameworkCore;
using DentalClinicAPI.Data;
using DentalClinicAPI.Repositories;
using DentalClinicAPI.Services;
using DentalClinicAPI.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuração de serviços
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configuração do pipeline de requisições HTTP
ConfigureApp(app);

app.Run();

// Método para configurar serviços
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Banco de dados
    services.AddDbContext<ClinicContext>(options =>
        options.UseOracle(configuration.GetConnectionString("OracleConnection")));

    // Repositórios e serviços
    RegisterRepositories(services);
    RegisterServices(services);

    // AutoMapper
    services.AddAutoMapper(typeof(AutoMapperProfile));

    // Autenticação JWT
    ConfigureAuthentication(services, configuration);

    // Controllers com Newtonsoft.Json
    services.AddControllers()
        .AddNewtonsoftJson();

    // Swagger com documentação
    ConfigureSwagger(services);
}

// Método para configurar o pipeline HTTP
void ConfigureApp(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dental Clinic API v1");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // Cria o banco de dados se não existir (apenas para desenvolvimento)
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ClinicContext>();
        dbContext.Database.EnsureCreated();
    }
}

// Método para registrar repositórios
void RegisterRepositories(IServiceCollection services)
{
    // Registramos o repositório usando implementação concreta
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));
    services.AddScoped(typeof(IWriteRepository<>), typeof(Repository<>));
}

// Método para registrar serviços
void RegisterServices(IServiceCollection services)
{
    // Registramos os serviços com suas implementações
    services.AddScoped<IClinicService, ClinicService>();
    services.AddScoped<IAuthService, AuthService>();

    // As interfaces derivadas usam a mesma implementação
    services.AddScoped<IAvailabilityService>(sp => sp.GetRequiredService<IClinicService>());
    services.AddScoped<IAppointmentService>(sp => sp.GetRequiredService<IClinicService>());

    // Adicionar o serviço de ML.NET
    services.AddScoped<IMLService, MLService>();
}

// Método para configurar autenticação
void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(configuration.GetSection("AppSettings:Token").Value ??
                             throw new InvalidOperationException("Token não configurado"))),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
}

// Método para configurar Swagger
void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Dental Clinic API", Version = "v1" });
        c.EnableAnnotations();

        // Configuração do arquivo XML de documentação
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        // Verifica se o arquivo XML existe antes de tentar incluí-lo
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        // Configuração JWT no Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header usando o esquema Bearer. Digite 'Bearer' [espaço] e seu token."
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}