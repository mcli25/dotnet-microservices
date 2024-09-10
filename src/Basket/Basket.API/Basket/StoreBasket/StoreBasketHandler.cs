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
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProto;
        private readonly ILogger<StoreBasketCommandHandler> _logger;

        public StoreBasketCommandHandler(IBasketRepository basketRepository, DiscountProtoService.DiscountProtoServiceClient discountProto,ILogger<StoreBasketCommandHandler> logger)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _discountProto = discountProto ?? throw new ArgumentNullException(nameof(discountProto)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Storing basket for user: {Username}", request.Cart.Username);
                
                foreach(var item in request.Cart.Items)
                {
                    try
                    {
                        var discountRequest = new DiscountGrpc.GetDiscountRequest { ProductName = item.ProductName };
                        _logger.LogInformation("Requesting discount for product: {ProductName}", item.ProductName);
                        var coupon = await _discountProto.GetDiscountAsync(discountRequest, cancellationToken: cancellationToken);
                        
                        if (coupon != null)
                        {
                            _logger.LogInformation("Discount applied: {Amount} for product: {ProductName}", coupon.Amount, item.ProductName);
                            item.Price -= coupon.Amount;
                        }
                        else
                        {
                            _logger.LogInformation("No discount found for product: {ProductName}", item.ProductName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error getting discount for product: {ProductName}", item.ProductName);
                    }
                }
                
                var storedCart = await _basketRepository.StoreBasket(request.Cart, cancellationToken);
                
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