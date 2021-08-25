using CSharpFunctionalExtensions;
using ProductCatalog.Dtos;
using ProductCatalogService.Models;
using System.Threading.Tasks;

namespace ProductCatalogService.Services
{
    public interface IProductService
    {
        Task<Result<CreateProductResponseDto>> CreateProductAsync(CreateProductRequestDto createProductRequestDto);
        Task<Result<Product>> GetProductByIdAsync(int productId);
    }
}