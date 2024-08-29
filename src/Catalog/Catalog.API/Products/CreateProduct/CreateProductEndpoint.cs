using Carter;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Mapster;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductRequest(string Name, string Description, List<string> Categories, string ImageFile, decimal Price);
    public record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/products", async (CreateProductRequest request, ISender sender, ILogger<CreateProductEndpoint> logger) =>
            {
                logger.LogInformation("Received request to create product: {ProductName}", request.Name);

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Results.BadRequest("Product name is required.");
                }

                try
                {
                    var command = new CreateProductCommand(
                        Guid.NewGuid(), 
                        request.Name,
                        request.Description,
                        request.Categories,
                        request.ImageFile,
                        request.Price
                    );

                    var result = await sender.Send(command);
                    var response = new CreateProductResponse(result.Id);

                    logger.LogInformation("Product created successfully with ID: {ProductId}", response.Id);

                    return Results.Created($"/products/{response.Id}", response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while creating product");
                    return Results.Problem("An error occurred while processing your request.", statusCode: 500);
                }
            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Create product")
            .WithDescription("Create a new product");
        }
    }
}