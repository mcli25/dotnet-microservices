
using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
    
    // Configure the database schema name if needed
    options.DatabaseSchemaName = "public";
    // Auto create and update the database on startup
    options.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.All;

    // Add any additional Marten configurations here
    // For example, to add specific document mappings:
    // options.RegisterDocumentType<YourDocumentType>();
})
.UseLightweightSessions();
if(builder.Environment.IsDevelopment()) 
{
    builder.Services.InitializeMartenWith<CatalogInitialData>();
}
// Add IDocumentSession as scoped
builder.Services.AddScoped<IDocumentSession>(sp => sp.GetRequiredService<IDocumentStore>().LightweightSession());
builder.Services.AddHealthChecks()
.AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();
// Configure the HTTP request pipeline

app.MapCarter();
app.UseStatusCodePages();
app.UseExceptionHandler(options => {});
app.UseHealthChecks("/health", new HealthCheckOptions{ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse});
app.Run();