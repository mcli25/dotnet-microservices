using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Models
{
    public class ShoppingCart
    {
        public string Username { get; set; } = default!;
        public List<ShoppingCartItem> Items { get; set; } = new();
        public decimal TotalCount => Items.Sum(x => x.Quantity * x.Price);

        public ShoppingCart(string username)
        {
            Username = username;
        }
        public ShoppingCart()
        {
            
        }
        
    }
}