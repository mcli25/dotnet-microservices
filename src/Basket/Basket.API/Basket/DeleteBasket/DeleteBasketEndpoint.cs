using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Basket.API.Basket.DeleteBasket
{

    public class DeleteBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{username}", async (string username, ISender sender, ILogger<DeleteBasketEndpoint> logger) =>
            {
                logger.LogInformation("Received delete basket request for username: {Username}", username);

                var result = await sender.Send(new DeleteBasketCommand(username));

                if (!result.IsSuccess)
                {
                    logger.LogWarning("Failed to delete basket for username: {Username}", username);
                    return Results.BadRequest("Failed to delete basket");
                }

                logger.LogInformation("Successfully deleted basket for username: {Username}", username);
                return Results.Ok(result);
            })
            .WithName("DeleteBasket")
            .Produces<DeleteBasketResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("Basket");
        }
    }
}