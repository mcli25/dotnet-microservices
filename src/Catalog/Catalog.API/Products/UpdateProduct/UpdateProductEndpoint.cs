
using Catalog.API.Models;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductRequest(string Name, string Description, List<string> Categories, string ImageFile, decimal Price);

    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products/{id}", async (Guid id, UpdateProductRequest request, ISender sender, ILogger<UpdateProductEndpoint> logger) =>
            {
                logger.LogInformation("Received request to update product with ID: {id}", id);

                try
                {
                    var command = new UpdateProductCommand(
                        id,
                        request.Name,
                        request.Description,
                        request.Categories,
                        request.ImageFile,
                        request.Price
                    );

                    var result = await sender.Send(command);

                    if (result.Updated)
                    {
                        logger.LogInformation("Successfully updated product with ID: {id}", id);
                        return Results.Ok(result.Product);
                    }
                    logger.LogWarning("Product with ID: {id} not found", id);
                    return Results.NotFound();
                    
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while updating product with ID: {id}", id);
                    return Results.Problem("An error occurred while updating the product", statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("UpdateProduct")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Update a product")
            .WithDescription("Updates an existing product with the provided information");
        }
    }

}