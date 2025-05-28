using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            var products = await productInterface.GetAllAsync();
            if (!products.Any())
            {
                return NotFound("No products found.");
            }

            var (_, list) = ProductConversion.FromEnity(null, products);
            return list!.Any() ? Ok(list) : NotFound("No products found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            var product = await productInterface.FindByIdAsync(id);
            if (product is null)
            {
                return NotFound("No products found");
            }
            var (_product, _) = ProductConversion.FromEnity(product, null);
            return _product is not null ? Ok(_product) : NotFound("No products found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var getEnity = ProductConversion.ToEnity(product);
            var response = await productInterface.CreateAsync(getEnity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var getEnity = ProductConversion.ToEnity(product);
            var response = await productInterface.UpdateAsync(getEnity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            var getEnity = ProductConversion.ToEnity(product);
            var response = await productInterface.DeleteAsync(getEnity);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}
