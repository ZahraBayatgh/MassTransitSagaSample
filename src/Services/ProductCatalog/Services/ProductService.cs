using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCatalog.Data;
using ProductCatalog.Dtos;
using ProductCatalogService.Models;
using System;
using System.Threading.Tasks;

namespace ProductCatalogService.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductCatalogDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ProductCatalogDbContext context,
            ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// This metode get product by product id.
        /// If the input id is not valid or an expiration occurs, a Failure will be returned.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<Result<Product>> GetProductByIdAsync(int productId)
        {
            try
            {
                // Check product id
                if (productId <= 0)
                    return Result.Failure<Product>("Product id is invalid.");

                // Get product by product id
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

                // Check product in db
                if (product == null)
                    return Result.Failure<Product>("Product not found.");

                return Result.Success(product);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Get {productId} product id failed. Exception detail:{ex.Message}");

                return Result.Failure<Product>($"Get {productId} product id failed.");
            }
        }

        public async Task<Result<CreateProductResponseDto>> CreateProductAsync(CreateProductRequestDto createProductRequestDto)
        {
            try
            {
                // Check product instance
                var productValidation = CheckProductInstance(createProductRequestDto);
                if (productValidation.IsFailure)
                    return Result.Failure<CreateProductResponseDto>(productValidation.Error);

                // Intialize product
                var product = new Product
                {
                    Name = createProductRequestDto.Name,
                    Photo = createProductRequestDto.Photo
                };

                // Add product in database
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                CreateProductResponseDto createProductResponseDto = new CreateProductResponseDto(product.Id, createProductRequestDto.Name, createProductRequestDto.InitialHand);
                return Result.Success(createProductResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Add {createProductRequestDto.Name} product failed. Exception detail:{ex.Message}");

                return Result.Failure<CreateProductResponseDto>($"Add {createProductRequestDto.Name} product failed.");
            }
        }

        /// <summary>
        /// This methode check a createProductRequestDto instance
        /// </summary>
        /// <param name="createProductRequestDto"></param>
        /// <returns></returns>
        private static Result CheckProductInstance(CreateProductRequestDto createProductRequestDto)
        {
            if (createProductRequestDto == null)
                return Result.Failure("CreateProductDto instance is invalid.");

            if (string.IsNullOrEmpty(createProductRequestDto.Name))
                return Result.Failure("Product name is empty.");

            return Result.Success();
        }

    }
}
