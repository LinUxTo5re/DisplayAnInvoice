using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using InvoiceApi.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Database Configuration
// -----------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Increase timeout to avoid Supabase pooler timeouts
        npgsqlOptions.CommandTimeout(180);
    })
);

// -----------------------------
// Add Controllers
// -----------------------------
builder.Services.AddControllers();

// -----------------------------
// Swagger (enable ALWAYS)
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Invoice API",
        Version = "v1",
        Description = "API for managing invoices and items",
        Contact = new OpenApiContact { Name = "Invoice API Support" }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

// -----------------------------
// CORS
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// -----------------------------
// Build App
// -----------------------------
var app = builder.Build();

// -----------------------------
// Enable Swagger (Dev + Prod)
// -----------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
    c.RoutePrefix = "swagger";
});

// -----------------------------
// Static Files + Middleware
// -----------------------------
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// SPA fallback (React/HTML)
app.MapFallbackToFile("index.html");

// -----------------------------
// BIND TO RENDER PORT
// -----------------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// -----------------------------
// REMOVE MIGRATION ON STARTUP
// (Supabase transaction pooler cannot handle migration load)
// -----------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<InvoiceDbContext>();

    try
    {
        // Only TEST connection
        if (context.Database.CanConnect())
        {
            Console.WriteLine("Connected to database successfully.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database connection failed at startup.");
    }
}

// -----------------------------
// Start App
// -----------------------------
app.Run();
