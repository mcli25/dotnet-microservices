using DiscountGrpc.Data;
using DiscountGrpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var connectionString = builder.Configuration.GetConnectionString("Database");
Console.WriteLine($"Connection string: {connectionString}");

builder.Services.AddDbContext<DiscountContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Ensure the database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DiscountContext>();
        Console.WriteLine($"Database path: {context.Database.GetDbConnection().DataSource}");
        Console.WriteLine($"Current directory: {Environment.CurrentDirectory}");
        Console.WriteLine($"Base directory: {AppContext.BaseDirectory}");
        
        // Check if the directory exists
        var dbPath = Path.GetDirectoryName(context.Database.GetDbConnection().DataSource);
        if (!Directory.Exists(dbPath))
        {
            Console.WriteLine($"Creating directory: {dbPath}");
            Directory.CreateDirectory(dbPath);
        }
        
        context.Database.EnsureCreated();
        Console.WriteLine("Database created successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while creating the database: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();