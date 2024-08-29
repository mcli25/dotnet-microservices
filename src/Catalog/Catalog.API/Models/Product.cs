using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public List<string> Categories { get; set; } = new();
        public string Description { get; set; }= default!;
        public string ImageFile { get; set; } = default!;
        public decimal Price { get; set; }

    }
}