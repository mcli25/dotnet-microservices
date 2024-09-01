using Catalog.API.Models;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(Guid Id, string Name, string Description, List<string> Categories, string ImageFile, decimal Price) : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Please specify an Id");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please specify a Name");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Please specify a Description");
            RuleFor(x => x.Categories).NotEmpty().WithMessage("Please specify at least one Category");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("Please specify an ImageFile");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Please specify a Price greater than zero");
        }
    }

    internal class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(IDocumentSession documentSession, ILogger<CreateProductCommandHandler> logger)
        {
            _documentSession = documentSession;
            _logger = logger;
        }

        public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateProductCommand for product: {ProductName}", request.Name);
            try
            {
                var existingProduct = await _documentSession.Query<Product>()
                .FirstOrDefaultAsync(p => p.Name == request.Name, cancellationToken);

                if (existingProduct != null)
                {
                    _logger.LogWarning("Attempting to create a product that already exists: {ProductName}", request.Name);
                    throw new Exception(request.Name);
                }
                
                var product = new Product
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    Categories = request.Categories ?? new List<string>(),
                    ImageFile = request.ImageFile,
                    Price = request.Price
                };

                _documentSession.Store(product);
                await _documentSession.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
                return new CreateProductResult(product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product: {ProductName}", request.Name);
                throw;
            }
        }
    }
}