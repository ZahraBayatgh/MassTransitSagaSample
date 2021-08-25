using CSharpFunctionalExtensions;
using ProductCatalogService.Dtos;
using ProductCatalogService.Models;
using System.Threading.Tasks;

namespace ProductCatalogService.Services
{
    public interface IProductService
    {
        Task<Result<Product>> GetProductByIdAsync(int productId);
        Task<Result<CreateProductResponseDto>> CreateProductAsync(CreateProductRequestDto createProductRequestDto);
        Task<Result> UpdateProductStatusAsync(UpdateProductStatusRequestDto updateProductStatusRequestDto);
        Task<Result> DeleteProductAsync(int productId);


    }
}