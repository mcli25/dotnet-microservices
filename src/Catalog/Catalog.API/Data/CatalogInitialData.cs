using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Models;
using Marten.Schema;

namespace Catalog.API.Data
{
    public class CatalogInitialData : IInitialData
    {
        public CatalogInitialData()
        {
        }

        public async Task Populate(IDocumentStore store, CancellationToken cancellation)
        {
            await using var session = store.LightweightSession();
            // Marten UPSERT will cater for existing records
            session.Store<Product>(GetPreconfiguredProducts());
            await session.SaveChangesAsync();
        }

        private static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>()
        {
            new Product()
            {
                Id = new Guid(),
                Name = "IPhone X",
                Description = "This phone is the company's biggest change to its flagship smartphone in years. It includes a borderless.",
                ImageFile = "product-1.png",
                Price = 950.00M,
                Categories = new List<string> { "Smart Phone" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Canon EOS R5",
                Description = "A professional-grade mirrorless camera with advanced features and high resolution.",
                ImageFile = "product-3.png",
                Price = 3899.99M,
                Categories = new List<string> { "Camera", "Photography", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "OnePlus 8 Pro",
                Description = "OnePlus 8 Pro is known for its high performance and smooth user experience at a reasonable price.",
                ImageFile = "product-4.png",
                Price = 899.99M,
                Categories = new List<string> { "Smart Phone", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Sony Alpha A7 III",
                Description = "Sony's full-frame mirrorless camera with stunning image quality and 4K video.",
                ImageFile = "product-5.png",
                Price = 1999.00M,
                Categories = new List<string> { "Camera", "Photography", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Huawei P40 Pro",
                Description = "Huawei P40 Pro features a stunning design and a powerful AI-driven camera system.",
                ImageFile = "product-6.png",
                Price = 900.00M,
                Categories = new List<string> { "Smart Phone", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "DJI Mavic Air 2",
                Description = "Compact and powerful drone with high-resolution camera and advanced flight capabilities.",
                ImageFile = "product-7.png",
                Price = 799.00M,
                Categories = new List<string> { "Drone", "Camera", "Photography" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Oppo Find X2 Pro",
                Description = "Oppo Find X2 Pro stands out with its stunning display and super-fast charging.",
                ImageFile = "product-8.png",
                Price = 999.00M,
                Categories = new List<string> { "Smart Phone", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Nikon Z6 II",
                Description = "Nikon's next-generation mirrorless camera offering versatile shooting capabilities.",
                ImageFile = "product-9.png",
                Price = 2499.99M,
                Categories = new List<string> { "Camera", "Photography", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "GoPro Hero 9",
                Description = "GoPro's latest action camera with improved stabilization and 5K video recording.",
                ImageFile = "product-10.png",
                Price = 399.00M,
                Categories = new List<string> { "Action Camera", "Photography", "Outdoors" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Asus ROG Phone 5",
                Description = "Asus ROG Phone 5 is a gaming powerhouse with high-end specs and a smooth display.",
                ImageFile = "product-11.png",
                Price = 999.00M,
                Categories = new List<string> { "Smart Phone", "Gaming", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Sony ZV-1",
                Description = "Compact vlogging camera designed for content creators, featuring high-quality video and audio.",
                ImageFile = "product-12.png",
                Price = 749.99M,
                Categories = new List<string> { "Camera", "Vlogging", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Realme X50 Pro",
                Description = "Realme X50 Pro offers 5G connectivity and premium features at a budget price.",
                ImageFile = "product-13.png",
                Price = 699.00M,
                Categories = new List<string> { "Smart Phone", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "DJI Osmo Pocket 2",
                Description = "A handheld stabilized camera perfect for vlogging and travel videos.",
                ImageFile = "product-14.png",
                Price = 349.99M,
                Categories = new List<string> { "Camera", "Vlogging", "Travel" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Fujifilm X-T4",
                Description = "Fujifilm's top mirrorless camera for enthusiasts and professionals with advanced features.",
                ImageFile = "product-15.png",
                Price = 1699.99M,
                Categories = new List<string> { "Camera", "Photography", "Electronics" }
            },
            new Product()
            {
                Id = Guid.NewGuid(),
                Name = "Samsung Galaxy S20",
                Description = "The Galaxy S20 is packed with the latest technology and supports 5G.",
                ImageFile = "product-2.png",
                Price = 850.00M,
                Categories = new List<string> { "Smart Phone", "Electronics" }
            }
        };
    }
}