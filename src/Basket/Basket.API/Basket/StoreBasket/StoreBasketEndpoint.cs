using System;
using System.Threading.Tasks;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Basket.API.Models;

namespace Basket.API.Basket.StoreBasket
{

    public class StoreBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket", async (ShoppingCart cart, ISender sender) =>
            {
                var command = new StoreBasketCommand(cart);
                var result = await sender.Send(command);

                return Results.Created($"/basket/{result.Username}", result);
            })
            .WithName("StoreBasket")
            .Produces<StoreBasketResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Basket");
        }
    }
}