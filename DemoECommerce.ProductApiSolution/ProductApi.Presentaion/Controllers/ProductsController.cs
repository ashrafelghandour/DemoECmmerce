using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Infrastructure.Repositories;
using ProductApiApplication.DTOs;
using ProductApiApplication.DTOs.ProductConverstion;
using ProductApiApplication.Interfaces;

namespace ProductApi.Presentaion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProduct product) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
        {
            //get all product from repo
            var products = await product.GetAllAsync();

            if (!products.Any())
                return NotFound("No products in database");
            
            //convert data from entity to DTo
            var (_, list) = ProductConversion.FromEintity(null, products);
            return list!.Any() ? Ok(products) : NotFound("Not found Products");
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            //get single product from repo
            var productDTO = await product.FindByIdAsync(id);

            if (productDTO is  null)
                return NotFound($"not found product with id {id}");

                 //conver form entity to Dto ant return
                var pro = ProductConversion.FromEintity(productDTO, null).Item1;
                return pro is null ? NotFound($"not found product with id {id}") : Ok(pro); 
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO productDTO)
        {


            //check model state is all data annotations is machs
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pro = ProductConversion.ToEntitiy(productDTO);

            var response = await product.CteateAsync(pro);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO dTO)
        {
            //check model state is all data annotations is machs
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pro = ProductConversion.ToEntitiy(dTO);
            var response = await product.UpdateAsync(pro);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO dTO)
        {
            //check model state is all data annotations is machs
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pro = ProductConversion.ToEntitiy(dTO);

            var response = await product.DeleteAsync(pro);
            return response.Flag is true ? Ok(response) : BadRequest(dTO);
        }


    }
}
