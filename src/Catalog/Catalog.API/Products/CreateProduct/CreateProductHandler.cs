
namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(Guid Id, string Name, string Description, List<string> Categories, string ImageFile, decimal Price) : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);

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
                var product = new Models.Product
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    Categories = request.Categories,
                    ImageFile = request.ImageFile,
                    Price = request.Price
                };

                //save the entity to the database
                _documentSession.Store(product);
                await _documentSession.SaveChangesAsync(cancellationToken);
                
                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);
                return new CreateProductResult(product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product");
                throw;
            }
        }
    }
}