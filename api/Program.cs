using System.Text;
using api.Data;
using api.Middleware;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT bearer token"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var connString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("CodeCraft")
    ?? throw new InvalidOperationException("Connection string is required");

builder.Services.AddDbContext<CodeCraftDbContext>(opt => opt.UseSqlServer(connString));
builder.Services.AddScoped<ITokenService, TokenService>();

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"] ?? "codecraft";
var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"] ?? "codecraft-ui";
var signingKey = Environment.GetEnvironmentVariable("JWT_SIGNING_KEY") ?? builder.Configuration["Jwt:SigningKey"]
    ?? throw new InvalidOperationException("JWT signing key is required");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new { status = "Server running" }));
app.MapGet("/all", async (CodeCraftDbContext db) =>
{
    var crafts = await db.Crafts.Where(c => c.IsPublic).ToListAsync();
    var response = new List<object>();
    foreach (var craft in crafts)
    {
        var likes = await db.CraftLikes.Where(l => l.CraftId == craft.Id).Select(l => l.Username).ToListAsync();
        response.Add(new { craft.CraftId, craft.Name, CreatedBy = craft.CreatedByUsername, craft.Js, craft.Css, craft.Html, craft.IsPublic, craft.IsFork, LikesCount = likes.Count, ViewsCount = await db.CraftViews.CountAsync(v => v.CraftId == craft.Id), LikedBy = string.Join(",", likes) });
    }
    return Results.Ok(response);
});
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CodeCraftDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        var devUser = new api.Entities.User
        {
            Username = "demo",
            Name = "Demo User",
            Email = "demo@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
        };
        db.Users.Add(devUser);

        db.Crafts.AddRange(
            new api.Entities.Craft
            {
                CraftId = "hello-world",
                Name = "Hello World",
                CreatedByUsername = devUser.Username,
                Html = "<h1>Hello CodeCraft</h1>",
                Css = "h1 { color: #0f766e; }",
                Js = "console.log('hello');",
                IsPublic = true
            },
            new api.Entities.Craft
            {
                CraftId = "card-sample",
                Name = "Card Sample",
                CreatedByUsername = devUser.Username,
                Html = "<div class='card'>Card</div>",
                Css = ".card { padding: 16px; border: 1px solid #ddd; border-radius: 8px; }",
                Js = "",
                IsPublic = true
            }
        );

        db.SaveChanges();
    }
}

app.Run();
