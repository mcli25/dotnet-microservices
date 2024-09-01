
using Catalog.API.Models;

namespace Catalog.API.Products.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<GetProductByIdResponse>;
    public record GetProductByIdResponse(Product? Product);
    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/{id}", async (Guid id, ISender sender, ILogger<GetProductByIdEndpoint> logger) =>
            {
                logger.LogInformation("Received request to get product. ProductId: {id}", id);

                try
                {
                    var query = new GetProductByIdQuery(id);
                    var response = await sender.Send(query);

                    if (response.Product == null)
                    {
                        logger.LogWarning("Product not found. ProductId: {id}", id);
                        return Results.NotFound();
                    }

                    return Results.Ok(response.Product);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while retrieving product");
                    return Results.Problem("An error occurred while retrieving the product", statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetProductById")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get product by ID")
            .WithDescription("Retrieves a specific product by its ID");
        }
    }
}