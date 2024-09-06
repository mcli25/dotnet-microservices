using System;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Logging;

namespace Basket.API.Data
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<BasketRepository> _logger;

        public BasketRepository(IDocumentSession documentSession, ILogger<BasketRepository> logger)
        {
            _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ShoppingCart> GetBasket(string username, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting basket for user: {Username}", username);
                var basket = await _documentSession.Query<ShoppingCart>()
                    .FirstOrDefaultAsync(b => b.Username == username, cancellationToken);

                if (basket == null)
                {
                    _logger.LogInformation("No basket found for user: {Username}", username);
                }

                return basket; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting basket for user: {Username}", username);
                throw;
            }
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Storing basket for user: {Username}", cart.Username);
                _documentSession.Store(cart);
                await _documentSession.SaveChangesAsync(cancellationToken);
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while storing basket for user: {Username}", cart.Username);
                throw;
            }
        }

        public async Task<bool> DeleteBasket(string username, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Deleting basket for user: {Username}", username);
                var basket = await _documentSession.Query<ShoppingCart>()
                    .FirstOrDefaultAsync(b => b.Username == username, cancellationToken);

                if (basket != null)
                {
                    _documentSession.Delete(basket);
                    await _documentSession.SaveChangesAsync(cancellationToken);
                    return true;
                }

                _logger.LogWarning("Basket not found for user: {Username}", username);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting basket for user: {Username}", username);
                throw;
            }
        }
    }
}