using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Data
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(string username, CancellationToken cancellationToken = default);
        Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken cancellationToken = default);
        Task<bool> DeleteBasket(string username, CancellationToken cancellationToken = default);
    }
}