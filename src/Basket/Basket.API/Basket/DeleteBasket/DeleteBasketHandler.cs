using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Basket.API.Basket.DeleteBasket
{
    public record DeleteBasketCommand(string Username) : ICommand<DeleteBasketResult>;
    
    public record DeleteBasketResult(bool IsSuccess, string Message);

    public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
    {
        public DeleteBasketCommandValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
        }
    }

    public class DeleteBasketHandler : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<DeleteBasketHandler> _logger;

        public DeleteBasketHandler(IBasketRepository basketRepository, ILogger<DeleteBasketHandler> logger)
        {
            _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DeleteBasketResult> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting basket for user: {Username}", request.Username);

                var isDeleted = await _basketRepository.DeleteBasket(request.Username, cancellationToken);

                if (isDeleted)
                {
                    _logger.LogInformation("Basket deleted successfully for user: {Username}", request.Username);
                    return new DeleteBasketResult(true, "Basket deleted successfully");
                }
                _logger.LogWarning("Basket not found for user: {Username}", request.Username);
                return new DeleteBasketResult(false, "Basket not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting basket for user: {Username}", request.Username);
                return new DeleteBasketResult(false, $"An error occurred: {ex.Message}");
            }
        }
    }
}