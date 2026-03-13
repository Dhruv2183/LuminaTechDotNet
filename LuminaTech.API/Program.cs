using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LuminaTech.Data;
using LuminaTech.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ----- Serilog -----
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// ----- Database -----
builder.Services.AddDbContext<LuminaTechDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=luminatech.db"));

// ----- Services -----
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// ----- Authentication -----
var jwtKey = builder.Configuration["Jwt:Key"] ?? "LuminaTech-Super-Secret-Key-2024-Change-In-Production-Please!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    // Fix SameSite cookie issue for HTTP localhost
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "YOUR_GOOGLE_CLIENT_ID";
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "YOUR_GOOGLE_CLIENT_SECRET";
    options.CallbackPath = "/signin-google";
    // Fix cookies for HTTP localhost
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.Events = new Microsoft.AspNetCore.Authentication.OAuth.OAuthEvents
    {
        OnRedirectToAuthorizationEndpoint = context =>
        {
            // Rewrite redirect_uri from https to http for local development
            var redirectUri = context.RedirectUri;
            if (redirectUri.Contains("redirect_uri=https%3A%2F%2Flocalhost"))
            {
                redirectUri = redirectUri.Replace(
                    "redirect_uri=https%3A%2F%2Flocalhost",
                    "redirect_uri=http%3A%2F%2Flocalhost");
            }
            context.Response.Redirect(redirectUri);
            return Task.CompletedTask;
        }
    };
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "LuminaTech",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "LuminaTech",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    // Read JWT from cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ----- Controllers + Swagger -----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LuminaTech API", Version = "v1" });
});

// ----- CORS -----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// ----- Ensure database created + migrate -----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LuminaTechDbContext>();
    db.Database.EnsureCreated();
}

// ----- Middleware -----
// Forward headers for OAuth behind proxies
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Force HTTP scheme for localhost only (fixes OAuth redirect_uri_mismatch)
if (app.Environment.IsDevelopment())
{
    app.Use((context, next) =>
    {
        // Ensure ASP.NET sees http scheme, not https
        context.Request.Scheme = "http";
        return next();
    });
}

// Swagger enabled in all environments for demo/portfolio
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LuminaTech API v1"));

app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Fallback to index.html for SPA-like navigation
app.MapFallbackToFile("index.html");

app.Run();
