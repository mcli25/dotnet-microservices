
namespace Catalog.API.Products.DeleteProduct
{
    public class DeleteProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/products/{id}", async (Guid id, ISender sender, ILogger<DeleteProductEndpoint> logger) =>
            {
                logger.LogInformation("Received request to delete product with ID: {id}", id);

                try
                {
                    var command = new DeleteProductCommand(id);
                    var response = await sender.Send(command);
                    if (response.Deleted)
                    {
                        logger.LogInformation("Successfully deleted product with ID: {id}", id);
                        return Results.NoContent();
                    }
                    logger.LogWarning("Product with ID: {id} not found for deletion", id);
                    return Results.NotFound($"Product with ID: {id} not found for deletion");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while deleting product with ID: {id}", id);
                    return Results.Problem("An error occurred while deleting the product", statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("DeleteProduct")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Delete a product")
            .WithDescription("Deletes an existing product with the provided ID");
        }
    }
}