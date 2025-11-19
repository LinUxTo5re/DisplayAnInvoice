using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using InvoiceApi.Data;

// Ensure Development environment
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Invoice API",
        Version = "v1",
        Description = "API for managing invoices and invoice items",
        Contact = new OpenApiContact
        {
            Name = "Invoice API Support"
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

//  CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API v1");
        c.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.UseStaticFiles();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<InvoiceDbContext>();
    
    try
    {
        context.Database.Migrate();
        SeedDatabase(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

static void SeedDatabase(InvoiceDbContext context)
{
    if (!context.Invoices.Any())
    {
        var invoice = new InvoiceApi.Models.Invoice
        {
            CustomerName = "John Doe",
            InvoiceDate = DateTime.UtcNow,
            TotalAmount = 0
        };

        context.Invoices.Add(invoice);
        context.SaveChanges();

        var items = new List<InvoiceApi.Models.InvoiceItem>
        {
            new InvoiceApi.Models.InvoiceItem
            {
                InvoiceId = invoice.InvoiceId,
                Name = "Widget A",
                Price = 19.99m,
                Quantity = 2
            },
            new InvoiceApi.Models.InvoiceItem
            {
                InvoiceId = invoice.InvoiceId,
                Name = "Widget B",
                Price = 29.99m,
                Quantity = 1
            },
            new InvoiceApi.Models.InvoiceItem
            {
                InvoiceId = invoice.InvoiceId,
                Name = "Service Fee",
                Price = 10.00m,
                Quantity = 1
            }
        };

        context.InvoiceItems.AddRange(items);
        invoice.TotalAmount = items.Sum(i => i.Price * i.Quantity);
        context.SaveChanges();
    }
}
