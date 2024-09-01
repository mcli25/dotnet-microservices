using Catalog.API.Models;
using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id, string Name, string Description, List<string> Categories, string ImageFile, decimal Price) : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool Updated, Product? Product);

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product ID is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please specify a Name");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Please specify a Description");
            RuleFor(x => x.Categories).NotEmpty().WithMessage("Please specify at least one Category");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("Please specify an ImageFile");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Please specify a Price greater than zero");
        }
    }

    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        public UpdateProductCommandHandler(IDocumentSession documentSession, ILogger<UpdateProductCommandHandler> logger)
        {
            _documentSession = documentSession;
            _logger = logger;
        }

        public async Task<UpdateProductResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateProductCommand for product ID: {ProductId}", request.Id);

            try
            {
                var product = await _documentSession.LoadAsync<Product>(request.Id, cancellationToken);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for ID: {ProductId}", request.Id);
                    return new UpdateProductResult(false, null);
                }

                // Update the product properties
                product.Name = request.Name;
                product.Description = request.Description;
                product.Categories = request.Categories ?? new List<string>();
                product.ImageFile = request.ImageFile;
                product.Price = request.Price;

                _documentSession.Update(product);
                await _documentSession.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Product updated successfully for ID: {ProductId}", request.Id);
                return new UpdateProductResult(true, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID: {ProductId}", request.Id);
                throw;
            }
        }
    }
}