
using Catalog.API.Models;

namespace Catalog.API.Products.GetProductsByCategory
{

    public record GetProductsByCategoryRequest(string Category) : IRequest<GetProductsByCategoryResponse>;
    public record GetProductsByCategoryResponse(IEnumerable<Product> Products, int TotalCount);
    public class GetProductsByCategoryEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products/category/{category}", async (string category, ISender sender, ILogger<GetProductsByCategoryEndpoint> logger) =>
            {
                logger.LogInformation("Received request to get products by category: {category}", category);

                try
                {
                    var query = new GetProductsByCategoryQuery(category);
                    var response = await sender.Send(query);
                    if (!response.Products.Any())
                    {
                        logger.LogWarning("No products found for category: {category}", category);
                        return Results.NotFound($"No products found for category: {category}");
                    }
                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while retrieving products for category: {category}", category);
                    return Results.Problem("An error occurred while retrieving products", statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetProductsByCategory")
            .Produces<GetProductsByCategoryResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get products by category")
            .WithDescription("Retrieves a list of products for a specific category");
        }
    }
}