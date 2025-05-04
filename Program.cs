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

// Configura��o de servi�os
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configura��o do pipeline de requisi��es HTTP
ConfigureApp(app);

app.Run();

// M�todo para configurar servi�os
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Banco de dados
    services.AddDbContext<ClinicContext>(options =>
        options.UseOracle(configuration.GetConnectionString("OracleConnection")));

    // Reposit�rios e servi�os
    RegisterRepositories(services);
    RegisterServices(services);

    // AutoMapper
    services.AddAutoMapper(typeof(AutoMapperProfile));

    // Autentica��o JWT
    ConfigureAuthentication(services, configuration);

    // Controllers com Newtonsoft.Json
    services.AddControllers()
        .AddNewtonsoftJson();

    // Swagger com documenta��o
    ConfigureSwagger(services);
}

// M�todo para configurar o pipeline HTTP
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

    // Cria o banco de dados se n�o existir (apenas para desenvolvimento)
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ClinicContext>();
        dbContext.Database.EnsureCreated();
    }
}

// M�todo para registrar reposit�rios
void RegisterRepositories(IServiceCollection services)
{
    // Registramos o reposit�rio usando implementa��o concreta
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>));
    services.AddScoped(typeof(IWriteRepository<>), typeof(Repository<>));
}

// M�todo para registrar servi�os
void RegisterServices(IServiceCollection services)
{
    // Registramos os servi�os com suas implementa��es
    services.AddScoped<IClinicService, ClinicService>();
    services.AddScoped<IAuthService, AuthService>();

    // As interfaces derivadas usam a mesma implementa��o
    services.AddScoped<IAvailabilityService>(sp => sp.GetRequiredService<IClinicService>());
    services.AddScoped<IAppointmentService>(sp => sp.GetRequiredService<IClinicService>());

    // Adicionar o servi�o de ML.NET
    services.AddScoped<IMLService, MLService>();
}

// M�todo para configurar autentica��o
void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
{
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(configuration.GetSection("AppSettings:Token").Value ??
                             throw new InvalidOperationException("Token n�o configurado"))),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
}

// M�todo para configurar Swagger
void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Dental Clinic API", Version = "v1" });
        c.EnableAnnotations();

        // Configura��o do arquivo XML de documenta��o
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        // Verifica se o arquivo XML existe antes de tentar inclu�-lo
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        // Configura��o JWT no Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header usando o esquema Bearer. Digite 'Bearer' [espa�o] e seu token."
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