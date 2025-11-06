using eCommerce.SharedLibrary;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.Conversrions;
using OrderApi.Application.DTO;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Serveces;
using OrderApi.Domin.Entites;

namespace OrderApi.Perisntation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrder order , IOrderService orderService) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            //get all Product from repo
            var Orders = await order.GetAllAsync();

            if (!Orders.Any())
                return NotFound("No Orders in database");

            //convert data from entity to DTo
            var (_, list) = OrderConversion.FromEntity(null, Orders);
            return list!.Any() ? Ok(Orders) : NotFound("Not found Orders");
        }
       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            //get single Order from repo
            var OrderDTO = await order.FindByIdAsync(id);

            if (OrderDTO is null)
                return NotFound($"not found Order with id {id}");

            //conver form entity to Dto ant return
            var pro = OrderConversion.FromEntity(OrderDTO, null).Item1;
            return pro is null ? NotFound($"not found Order with id {id}") : Ok(pro);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(OrderDTO OrderDTO)
        {


            //check model state is all data annotations is machs
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var or = OrderConversion.ToEntity(OrderDTO);

            var response = await order.CteateAsync(or);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(OrderDTO dTO)
        {
            //check model state is all data annotations is machs
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pro = OrderConversion.ToEntity(dTO);
            var response = await order.UpdateAsync(pro);
            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(OrderDTO dTO)
        {
            //check model state is all data annotations is machs
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pro = OrderConversion.ToEntity(dTO);

            var response = await order.DeleteAsync(pro);
            return response.Flag is true ? Ok(response) : BadRequest(dTO);
        }

        [HttpGet("Client/{clinetid:int}")]
        public async Task<ActionResult<IEnumerator<OrderDTO>>> GetClientOrder(int clinetid)
        {
            

                if (clinetid < 1) return BadRequest("Invalid data provided");

                var order = orderService.GetOrdersByClientId(clinetid);
                return order is not null ? Ok(order) : NotFound(order);
            
           

        }
        [HttpGet("Clientv2/{clinetid:int}")]
        public async Task<ActionResult<IEnumerator<OrderDTO>>> GetClientOrderV2(int clinetid)
        {


            if (clinetid < 1) return BadRequest("Invalid data provided");

            var ordr = order.GetAllAsync().Result.Where(o=>o.ClientId == clinetid);
            var dto = OrderConversion.FromEntity(null, ordr).Item2;
            return ordr is not null ? Ok(dto) : NotFound(dto);



        }
        [HttpGet("details/{orderid:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderid)
        {
           
                if (orderid < 1) return BadRequest("Invalid data provided");

                var details = await orderService.GetOrderDetailsByOrderId(orderid);
          


            return  details.OrderID <= 0 ? NotFound("Not order founf") : Ok(details);


        }


    }
} 
