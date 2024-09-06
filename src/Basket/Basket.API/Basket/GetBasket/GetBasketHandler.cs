using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Basket.GetBasket
{
    // public record GetBasketQuery(string Username) : IQuery<GetBasketResult>;
    // public record GetBasketResult(ShoppingCart cart);
    public class GetBasketHandler : IRequestHandler<GetBasketQuery, GetBasketResponse>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<GetBasketHandler> _logger;

        public GetBasketHandler(IBasketRepository basketRepository, ILogger<GetBasketHandler> logger)
        {
            _basketRepository = basketRepository;
            _logger = logger;
        }

        public async Task<GetBasketResponse> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetBasket(request.Username, cancellationToken);

            if (basket == null)
            {
                _logger.LogInformation("Basket not found for user: {Username}", request.Username);
                return new GetBasketResponse(null); 
            }

            return new GetBasketResponse(basket);
        }
    }
}