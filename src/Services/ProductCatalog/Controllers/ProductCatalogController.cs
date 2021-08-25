using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Dtos;
using ProductCatalogService.Services;
using System.Threading.Tasks;

namespace ProductCatalog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductCatalogController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductOrchestratorService _productOrchestratorService;

        public ProductCatalogController(IProductService productService,
                                        IProductOrchestratorService productOrchestratorService)
        {
            _productService = productService;
            _productOrchestratorService = productOrchestratorService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            // Get product by product id
            var product = await _productService.GetProductByIdAsync(id);
            if (product.IsSuccess)
            {
                return Ok(product.Value);
            }

            return BadRequest(product.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(CreateProductRequestDto createProductRequestDto)
        {
            // Create product and inventory transaction
            var createProductResponse = await _productOrchestratorService.CreateProductAndPublishEvent(createProductRequestDto, HttpContext.TraceIdentifier);

            if (createProductResponse.IsSuccess)
            {
                return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createProductResponse.Value }, null);
            }

            return BadRequest(createProductResponse.Error);
        }
    }
}
