
using Catalog.API.Models;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsQuery(int PageNumber = 1, int PageSize = 10) : IQuery<GetProductsResult>;
    public record GetProductsResult(IEnumerable<Product> Products, int TotalCount);

    internal class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsResult>
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<GetProductsQueryHandler> _logger;
        public GetProductsQueryHandler(IDocumentSession documentSession, ILogger<GetProductsQueryHandler> logger)
        {
            _documentSession = documentSession;
            _logger = logger;
        }

        public async Task<GetProductsResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProductsRequest: Page {PageNumber}, Size {PageSize}", request.PageNumber, request.PageSize);
            try
            {
                var query = _documentSession.Query<Product>();
                var totalCount = await query.CountAsync(cancellationToken);

                var products = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {ProductCount} products out of {TotalCount}", products.Count, totalCount);

                return new GetProductsResult(products, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products");
                throw;
            }
        }

    }
}