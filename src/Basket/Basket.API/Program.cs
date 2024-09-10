using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

});
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    // options.InstanceName = "BasketApp"; 
});


builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCarter();

builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("DefaultConnection")!);
    
    // Configure the database schema name if needed
    options.DatabaseSchemaName = "public";
    // Auto create and update the database on startup
    options.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.All;
    options.Schema.For<ShoppingCart>().Identity(x => x.Username);

})
.UseLightweightSessions();

//Grpc
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});

// Add IDocumentSession as scoped
builder.Services.AddScoped<IDocumentSession>(sp => sp.GetRequiredService<IDocumentStore>().LightweightSession());
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("RedisConnection")!);


var app = builder.Build();
// Configure the HTTP request pipeline

app.MapCarter();
app.UseStatusCodePages();
app.UseExceptionHandler(options => {});

app.UseHealthChecks("/health", new HealthCheckOptions{ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse});
app.Run();
