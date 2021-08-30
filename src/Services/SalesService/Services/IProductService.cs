using CSharpFunctionalExtensions;
using SalesService.Dtos;
using SalesService.Models;
using System.Threading.Tasks;

namespace SalesService.Services
{
    public interface IProductService
    {
        Task<Result<Product>> GetProductByIdAsync(int productId);
        Task<Result<Product>> GetProductByNameAsync(string productName);
        Task<Result<int>> CreateProductAsync(CreateProductRequestDto createProductRequestDto);
        Task<Result> DeleteProductByNameAsync(string productName);
        Task<Result> UpdateProductCountAsync(UpdateProductCountDto updateProductDto);
        Task<Result> CancelChangeProductCountAsync(CancelChangeProductCountDto createProductDto);
    }
}