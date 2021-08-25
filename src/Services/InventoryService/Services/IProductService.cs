using CSharpFunctionalExtensions;
using InventoryService.Dtos;
using InventoryService.Models;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public interface IProductService
    {
        Task<Result<Product>> GetProductByIdAsync(int productId);
        Task<Result<CreateProductResponseDto>> CreateProductAsync(ProductRequestDto productRequestDto);
    }
}