using System.Text;
using DevReview.API.BackgroundServices;
using DevReview.API.Configuration;
using DevReview.API.Extensions;
using DevReview.API.Middleware;
using DevReview.Domain.Enums;
using DevReview.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddControllers(options =>
    {
        options.Conventions.Add(new ApiRouteConvention());
    });
    builder.Services.AddProblemDetails();
    builder.Services.Configure<SanitizationOptions>(
        builder.Configuration.GetSection(SanitizationOptions.SectionName));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "DevReview API",
            Version = "v1",
            Description = "Code review and mentorship platform — versioned REST API at /api/v1"
        });

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (System.IO.File.Exists(xmlPath))
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

        var jwtScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter 'Bearer' [space] and then your valid token. Example: 'Bearer {token}'",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        };

        c.AddSecurityDefinition("Bearer", jwtScheme);
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { jwtScheme, Array.Empty<string>() }
        });
    });

    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureLayer(builder.Configuration);
    builder.Services.AddHostedService<ClaimTimeoutBackgroundService>();

    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
        options.AddPolicy("MentorOnly", policy => policy.RequireRole(UserRoles.Mentor));
        options.AddPolicy("AdminOnly", policy => policy.RequireRole(UserRoles.Admin));
    });

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

    var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
        ?? ["http://localhost:5173"];

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("DevReviewPolicy", policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseMiddleware<ExceptionMiddleware>();
    app.UseMiddleware<InputSanitizationMiddleware>();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevReview API v1");
    });

    app.UseCors("DevReviewPolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("DevReview API starting — routes under /api/v1");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "DevReview API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
