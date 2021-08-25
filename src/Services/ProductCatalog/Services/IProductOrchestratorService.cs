using CSharpFunctionalExtensions;
using ProductCatalog.Dtos;
using System.Threading.Tasks;

namespace ProductCatalogService.Services
{
    public interface IProductOrchestratorService
    {
        Task<Result<int>> CreateProductAndPublishEvent(CreateProductRequestDto createProductRequestDto, string correlationId);
    }
}