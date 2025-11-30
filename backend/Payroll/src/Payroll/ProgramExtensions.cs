using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Payroll;

public static class ProgramExtensions
{
    public static void AddConfiguredServices(this WebApplicationBuilder builder)
    {
        builder.AddSwagger();
        builder.AddDatabase();
        builder.Services.AddValidatorsFromAssembly(typeof(ProgramExtensions).Assembly);
        builder.AddCors();
        builder.AddJwtAuthentication();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
    }

    private static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    private static void AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<PayrollDbContext>(options =>
        {
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
                .UseSnakeCaseNamingConvention();
        });
    }

    private static void AddJwtAuthentication(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = Jwt.SecurityKey(builder.Configuration["Jwt:Key"]!),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx => Task.CompletedTask,
                };
            });

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddTransient<Jwt>();

        builder
            .Services.AddAuthorizationBuilder()
            .AddPolicy("role:admin", p => p.RequireRole(Roles.Admin))
            .AddPolicy("role:employee", p => p.RequireRole(Roles.Employee));

        builder.Services.AddAuthorization();
    }

    private static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                name: "AllowFrontend",
                policy =>
                {
                    var env = builder.Environment.EnvironmentName;

                    if (env == "Development" || env == "Docker")
                    {
                        // Local Nuxt running on port 3000
                        policy
                            .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders("Content-Type", "Accept", "Cache-Control")
                            .AllowCredentials();
                    }
                    else // Production
                    {
                        // Replace with deployed frontend domain
                        policy
                            .WithOrigins("https://app.yourcompany.com")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    }
                }
            );
        });
    }
}
