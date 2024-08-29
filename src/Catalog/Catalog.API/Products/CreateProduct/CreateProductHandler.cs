using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(Guid Id, string Name, string Description, List<string> Categories, string ImageFile, decimal Price) : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);

    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(ILogger<CreateProductCommandHandler> logger)
        {
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