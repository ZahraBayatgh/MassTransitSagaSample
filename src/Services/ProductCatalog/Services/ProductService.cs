using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductCatalog.Data;
using ProductCatalogService.Dtos;
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
        /// This method updates the product status.
        /// If the input updateProductDto is not valid or an expiration occurs, a Failure will be returned.
        /// If product count be lesser than DecreaseCount, a Failure will be returned.
        /// </summary>
        /// <param name="updateProductStatusRequestDto"></param>
        /// <returns></returns>
        public async Task<Result> UpdateProductStatusAsync(UpdateProductStatusRequestDto updateProductStatusRequestDto)
        {
            try
            {
                // Check updateProductDto instance
                var updateProductDtoValidation = CheckUpdateProductStatusRequestDtoInstance(updateProductStatusRequestDto);
                if (updateProductDtoValidation.IsFailure)
                    return Result.Failure<int>(updateProductDtoValidation.Error);

                // Get product
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Name == updateProductStatusRequestDto.Name);

                // Check product
                if (product == null)
                    return Result.Failure($"Product not found.");

                // Decrease product count
                product.ProductStatus = (ProductStatus)updateProductStatusRequestDto.ProductStatus;
                await _context.SaveChangesAsync();

                return Result.Success();

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Update {updateProductStatusRequestDto.Name} product failed. Exception detail:{ex.Message}");

                return Result.Failure($"Update {updateProductStatusRequestDto.Name} product failed.");
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

        /// <summary>
        /// This methode check a updateProductStatusRequestDto instance
        /// </summary>
        /// <param name="updateProductStatusRequestDto"></param>
        /// <returns></returns>
        private static Result CheckUpdateProductStatusRequestDtoInstance(UpdateProductStatusRequestDto updateProductStatusRequestDto)
        {
            if (updateProductStatusRequestDto == null)
                return Result.Failure("UpdateProductStatusRequestDto instance is invalid.");

            if (string.IsNullOrEmpty(updateProductStatusRequestDto.Name))
                return Result.Failure("UpdateProductStatusRequestDto name is empty.");

            if (!Enum.IsDefined(typeof(ProductStatus), (ProductStatus)updateProductStatusRequestDto.ProductStatus))
                return Result.Failure("UpdateProductStatusRequestDto ProductStatus is invaild.");

            return Result.Success();
        }
        /// <summary>
        /// This method delete a Product to the table.
        /// If the input productId is not valid or an expiration occurs, a Failure will be returned.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<Result> DeleteProductAsync(int productId)
        {
            try
            {
                // Check product id
                if (productId <= 0)
                    return Result.Failure($"Product id is zero.");

                // Get product by product id
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
                if (product == null)
                    return Result.Failure($"Product id is invalid.");

                // Remove product
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Delete product with {productId} id failed. Exception detail:{ex.Message}");

                return Result.Failure($"Delete product with {productId} id failed.");
            }
        }
    }
}
