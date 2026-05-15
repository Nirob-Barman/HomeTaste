using HomeTaste.Application.Authorization;
using HomeTaste.Application.DependencyInjection;
using HomeTaste.Infrastructure.DependencyInjection;
using HomeTaste.Infrastructure.Identity.Seed;
using HomeTaste.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSharedService(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly,             p => p.RequireRole("Admin"));
    options.AddPolicy(Policies.CustomerOnly,          p => p.RequireRole("Customer"));
    options.AddPolicy(Policies.DeliveryPersonnelOnly, p => p.RequireRole("DeliveryPersonnel"));
    options.AddPolicy(Policies.AdminOrDelivery,       p => p.RequireRole("Admin", "DeliveryPersonnel"));
    options.AddPolicy(Policies.AdminOrCustomer,       p => p.RequireRole("Admin", "Customer"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy
            .SetIsOriginAllowed(origin => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "HomeTaste API",
        Version = "v1",
        Description = "API for HomeTaste - A homemade food delivery platform."
    });

    var xmlFile = Path.Combine(AppContext.BaseDirectory, "HomeTaste.xml");
    options.IncludeXmlComments(xmlFile);

    // JWT Bearer security definition
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Enter your JWT token. Example: eyJhbGci..."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();



app.UseCustomMiddlewares();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ClientPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapNotificationHub();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedDefaultRolesAndAdminAsync(services);
}

app.Run();
