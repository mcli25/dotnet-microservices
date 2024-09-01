using Catalog.API.Models;

namespace Catalog.API.Products.GetProductsByCategory
{
    public record GetProductsByCategoryQuery(string Category) : IRequest<GetProductsByCategoryResult>;
    public record GetProductsByCategoryResult(IEnumerable<Product> Products, int TotalCount);

    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<GetProductsByCategoryQueryHandler> _logger;

        public GetProductsByCategoryQueryHandler(IDocumentSession documentSession, ILogger<GetProductsByCategoryQueryHandler> logger)
        {
            _documentSession = documentSession; 
            _logger = logger;
        }

        public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProductsByCategoryQuery for category: {Category}", request.Category);

            try
            {
                var query = _documentSession.Query<Product>()
                    .Where(p => p.Categories.Contains(request.Category));
                var products = await query.ToListAsync(cancellationToken);
                var totalCount = products.Count;

                _logger.LogInformation("Retrieved {Count} products for category: {Category}", totalCount, request.Category);

                return new GetProductsByCategoryResult(products, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products for category: {Category}", request.Category);
                throw;
            }
        }
    }
}