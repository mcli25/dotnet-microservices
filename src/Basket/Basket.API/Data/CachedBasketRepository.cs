using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Data
{
    public class CachedBasketRepository : IBasketRepository
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<BasketRepository> _logger;
        private readonly IBasketRepository _repository;
        private readonly IDistributedCache _cache;

        public CachedBasketRepository(IDocumentSession documentSession, ILogger<BasketRepository> logger, IBasketRepository repository, IDistributedCache cache)
        {
            _documentSession = documentSession;
            _logger = logger;
            _repository = repository;
            _cache = cache;
        }

        public async Task<ShoppingCart> GetBasket(string username, CancellationToken cancellationToken = default)
        {

            var cachedBasket = await _cache.GetStringAsync(username, cancellationToken);
            if (!string.IsNullOrEmpty(cachedBasket))
            {
                try
                {
                    return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached basket for user {Username}. Fetching from repository.", username);
                }
            }
            
            var basket = await _repository.GetBasket(username, cancellationToken);
            if (basket != null)
            {
                await _cache.SetStringAsync(
                    username,
                    JsonSerializer.Serialize(basket),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    },
                    cancellationToken
                );
            }

            return basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken cancellationToken = default)
        {
            await _repository.StoreBasket(cart, cancellationToken);
            await _cache.SetStringAsync(cart.Username, JsonSerializer.Serialize(cart), cancellationToken);
            
            return cart;
        }


        public async Task<bool> DeleteBasket(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            }

            try
            {
                bool IsSuccess = await _repository.DeleteBasket(username, cancellationToken);
                if (!IsSuccess)
                {
                    _logger.LogWarning("Failed to delete basket for user {Username} from repository", username);
                    return false;
                }

                await _cache.RemoveAsync(username, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting basket for user {Username}", username);
                return false;
            }
        }

    }
}