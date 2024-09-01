using Catalog.API.Models;

namespace Catalog.API.Products.DeleteProduct
{
    public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;
    public record DeleteProductResult(bool Deleted);

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Please specify an Id");
        }
    }

    public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, DeleteProductResult>
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(IDocumentSession documentSession, ILogger<DeleteProductCommandHandler> logger)
        {
            _documentSession = documentSession;
            _logger = logger;
        }

        public async Task<DeleteProductResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteProductCommand for product ID: {ProductId}", request.Id);

            try
            {
                var product = await _documentSession.LoadAsync<Product>(request.Id, cancellationToken);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for deletion. ID: {ProductId}", request.Id);
                    return new DeleteProductResult(false);
                }

                _documentSession.Delete<Product>(request.Id);
                await _documentSession.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Product deleted successfully. ID: {ProductId}", request.Id);
                return new DeleteProductResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product. ID: {ProductId}", request.Id);
                throw;
            }
        }
    }
}