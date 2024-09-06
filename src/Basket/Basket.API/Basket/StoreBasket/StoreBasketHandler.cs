using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string Username);

    public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketCommandValidator()
        {
            RuleFor(x => x.Cart).NotNull().WithMessage("Cart cannot be null");
            When(x => x.Cart != null, () =>
            {
                RuleFor(x => x.Cart.Username).NotEmpty().WithMessage("Username is required");
                RuleFor(x => x.Cart.Items).NotEmpty().WithMessage("Cart must contain at least one item");
            });
        }
    }

    public class StoreBasketCommandHandler : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<StoreBasketCommandHandler> _logger;

        public StoreBasketCommandHandler(IBasketRepository basketRepository, ILogger<StoreBasketCommandHandler> logger)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Storing basket for user: {Username}", request.Cart.Username);

                var storedCart = await _basketRepository.StoreBasket(request.Cart, cancellationToken);

                _logger.LogInformation("Basket stored successfully for user: {Username}", storedCart.Username);
                return new StoreBasketResult(storedCart.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while storing basket for user: {Username}", request.Cart.Username);
                throw;
            }
        }
    }
}