
using Catalog.API.Models;


namespace Catalog.API.Products.GetProducts
{
    public record GetProductsRequest(int PageNumber, int PageSize) : IRequest<GetProductsResponse>;
    public record GetProductsResponse(IEnumerable<Product> Products, int TotalCount);

    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async (int pageNumber, int pageSize, ISender sender, ILogger<GetProductsEndpoint> logger) =>
            {
                logger.LogInformation("Received request to get products. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

                try
                {
                    var request = new GetProductsQuery(pageNumber, pageSize);
                    var response = await sender.Send(request);

                    logger.LogInformation("Retrieved {ProductCount} products out of {TotalCount}", response.Products.Count(), response.TotalCount);

                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while retrieving products");
                    return Results.Problem("An error occurred while retrieving products", statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetProducts")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Get all products")
            .WithDescription("Retrieves a paginated list of all products");
        }
    }
}