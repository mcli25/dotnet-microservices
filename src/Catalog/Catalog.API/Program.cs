
var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddCarter();
builder.Services.AddMediatR(config => {
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

builder.Services.AddMarten(options =>
{
    // Replace with your actual connection string
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

// Add IDocumentSession as scoped
builder.Services.AddScoped<IDocumentSession>(sp => sp.GetRequiredService<IDocumentStore>().LightweightSession());

var app = builder.Build();

// Configure the HTTP request pipeline
app.MapCarter();

app.Run();