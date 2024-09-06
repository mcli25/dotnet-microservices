using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Basket.API.Basket.GetBasket
{

    public record GetBasketQuery(string Username) : IRequest<GetBasketResponse>;
    public record GetBasketResponse(ShoppingCart ? Cart);
    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{username}", async (string username, ISender sender, ILogger<GetBasketEndpoint> logger) =>
            {
                var result = await sender.Send(new GetBasketQuery(username));
                if (result.Cart == null)
                {
                    logger.LogWarning("Cart not found.");
                    return Results.NotFound();
                }

                return Results.Ok(result);
            })
            .WithName("GetBasket")
            .Produces<GetBasketResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Basket");
        }
    }

}