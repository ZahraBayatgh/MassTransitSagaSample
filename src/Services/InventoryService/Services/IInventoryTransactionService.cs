using CSharpFunctionalExtensions;
using InventoryService.Dtos;
using InventoryService.Models;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public interface IInventoryTransactionService
    {
        Task<Result<InventoryTransaction>> GetInventoryTransactionsByProductIdAsync(int productId);
        Task<Result<InventoryTransaction>> CreateInventoryTransactionAsync(InventoryTransactionRequestDto inventoryTransactionDto);
        Task<Result> DeleteInventoryTransactionAsync(int inventoryTransactionId);
    }
}