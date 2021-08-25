using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesService.Data;
using SalesService.Dtos;
using SalesService.Models;
using System;
using System.Threading.Tasks;

namespace SalesService.Services
{
    public class ProductService : IProductService
    {
        private readonly SaleDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(SaleDbContext context,
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
                    return Result.Failure<Product>($"Product id is invalid.");

                // Get product by product id
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);

                // Check product in db
                if (product == null)
                    return Result.Failure<Product>($"Product not found.");

                return Result.Success(product);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Get {productId} product id failed. Exception detail:{ex.Message}");

                return Result.Failure<Product>($"Get {productId} product id failed.");
            }
        }

        /// <summary>
        /// This metode get product by product name.
        /// If the input name is not valid or an expiration occurs, a Failure will be returned.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<Result<Product>> GetProductByNameAsync(string productName)
        {
            try
            {
                // Check product name
                if (string.IsNullOrEmpty(productName))
                    return Result.Failure<Product>($"Product name is null.");

                // Get product by product name
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Name == productName);

                // Check product in db
                if (product == null)
                    return Result.Failure<Product>($"Product not found.");

                return Result.Success(product);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Get {productName} product name failed. Exception detail:{ex.Message}");

                return Result.Failure<Product>($"Get {productName} product name failed.");
            }
        }

        /// <summary>
        /// This method adds a Product to the table.
        /// If the input createProductRequestDto is not valid or an expiration occurs, a Failure will be returned.
        /// </summary>
        /// <param name="createProductRequestDto"></param>
        /// <returns></returns>
        public async Task<Result<int>> CreateProductAsync(CreateProductRequestDto createProductRequestDto)
        {
            try
            {
                // Check product instance
                var productValidation = CheckProductInstance(createProductRequestDto);
                if (productValidation.IsFailure)
                    return Result.Failure<int>(productValidation.Error);

                // Intialize product
                var product = new Product
                {
                    Name = createProductRequestDto.Name,
                    OnHand = createProductRequestDto.Count
                };

                // Add product in database
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                return Result.Success(product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Add {createProductRequestDto.Name} product failed. Exception detail:{ex.Message}");

                return Result.Failure<int>($"Add {createProductRequestDto.Name} product failed.");
            }
        }

        /// <summary>
        /// This method updates the product count.
        /// If the input updateProductDto is not valid or an expiration occurs, a Failure will be returned.
        /// If product count be lesser than DecreaseCount, a Failure will be returned.
        /// </summary>
        /// <param name="updateProductDto"></param>
        /// <returns></returns>
        public async Task<Result> UpdateProductCountAsync(UpdateProductCountDto updateProductDto)
        {
            try
            {
                // Check updateProductDto instance
                var updateProductDtoValidation = CheckUpdateProductDtoInstance(updateProductDto);
                if (updateProductDtoValidation.IsFailure)
                    return Result.Failure<int>(updateProductDtoValidation.Error);

                // Get product
                var product = await _context.Products.FirstOrDefaultAsync(x => x.Name == updateProductDto.Name);

                // Check that the product count value is not less than the DecreaseCount.
                if (product.OnHand < updateProductDto.Quantity)
                    return Result.Failure<int>($"{product.Name} product count lesser than DecreaseCount.");

                // Decrease product count
                product.OnHand -= updateProductDto.Quantity;
                await _context.SaveChangesAsync();

                return Result.Success();

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Update {updateProductDto.Name} product failed. Exception detail:{ex.Message}");

                return Result.Failure($"Update {updateProductDto.Name} product failed.");
            }
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
        /// <summary>
        /// This method roll back product count.
        /// If the input cancelChangeProductCount is not valid or an expiration occurs, a Failure will be returned.
        /// </summary>
        /// <param name="cancelChangeProductCount"></param>
        /// <returns></returns>
        public async Task<Result> CancelChangeProductCountAsync(CancelChangeProductCountDto cancelChangeProductCount)
        {
            try
            {
                // Check cancelChangeProductCount instance
                var cancelChangeProductCountValidation = CheckUpdateProductDtoInstance(cancelChangeProductCount);
                if (cancelChangeProductCountValidation.IsFailure)
                    return Result.Failure<int>(cancelChangeProductCountValidation.Error);

                // Get product
                var result = await _context.Products.FirstOrDefaultAsync(x => x.Name == cancelChangeProductCount.Name);

                //Roll back product count
                result.OnHand += cancelChangeProductCount.DecreaseCount;
                await _context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Cancel {cancelChangeProductCount.Name} product failed. Exception detail:{ex.Message}");

                return Result.Failure($"Cancel {cancelChangeProductCount.Name} product failed.");
            }
        }

        /// <summary>
        /// This methode check a createProductDto instance
        /// </summary>
        /// <param name="createProductDto"></param>
        /// <returns></returns>
        private static Result CheckProductInstance(CreateProductRequestDto createProductDto)
        {
            if (createProductDto == null)
                return Result.Failure($"CreateProductDto instance is invalid.");

            if (string.IsNullOrEmpty(createProductDto.Name))
                return Result.Failure($"Product name is empty.");

            if (createProductDto.Count <= 0)
                return Result.Failure($"Product count is invaild.");

            return Result.Success();
        }

        /// <summary>
        /// This methode check a updateProductDto instance
        /// </summary>
        /// <param name="updateProductDto"></param>
        /// <returns></returns>
        private static Result CheckUpdateProductDtoInstance(UpdateProductCountDto updateProductDto)
        {
            if (updateProductDto == null)
                return Result.Failure($"UpdateProductDto instance is invalid.");

            if (string.IsNullOrEmpty(updateProductDto.Name))
                return Result.Failure($"UpdateProductDto name is empty.");

            if (updateProductDto.Quantity <= 0)
                return Result.Failure($"UpdateProductDto DecreaseCount is invaild.");

            return Result.Success();
        }

        /// <summary>
        /// This methode check a cancelChangeProductCountDto instance
        /// </summary>
        /// <param name="cancelChangeProductCountDto"></param>
        /// <returns></returns>
        private static Result CheckUpdateProductDtoInstance(CancelChangeProductCountDto cancelChangeProductCountDto)
        {
            if (cancelChangeProductCountDto == null)
                return Result.Failure($"CancelChangeProductCountDto instance is invalid.");

            if (string.IsNullOrEmpty(cancelChangeProductCountDto.Name))
                return Result.Failure($"CancelChangeProductCountDto name is empty.");

            if (cancelChangeProductCountDto.DecreaseCount <= 0)
                return Result.Failure($"CancelChangeProductCountDto DecreaseCount is invaild.");

            return Result.Success();
        }

    }
}
