
using Catalog.API.Models;


namespace Catalog.API.Products.GetProductById
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse>
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;

        public GetProductByIdQueryHandler(IDocumentSession documentSession, ILogger<GetProductByIdQueryHandler> logger)
        {
            _documentSession = documentSession;
            _logger = logger;
        }

        public async Task<GetProductByIdResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProductByIdQuery for product ID: {ProductId}", request.Id);

            try
            {
                var product = await _documentSession.LoadAsync<Product>(request.Id, cancellationToken);
                if (product == null)
                {
                    _logger.LogWarning("Product not found for ID: {ProductId}", request.Id);
                }
                else
                {
                    _logger.LogInformation("Product retrieved successfully for ID: {ProductId}", request.Id);
                }

                return new GetProductByIdResponse(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product with ID: {ProductId}", request.Id);
                throw;
            }
        }
    }
}