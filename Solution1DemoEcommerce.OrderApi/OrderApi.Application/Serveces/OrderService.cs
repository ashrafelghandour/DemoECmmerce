using System.Net.Http.Json;
using OrderApi.Application.Conversrions;
using OrderApi.Application.DTO;
using OrderApi.Application.Interfaces;
using OrderApi.Domin.Entites;
using Polly.Registry;

namespace OrderApi.Application.Serveces
{
    public class OrderService(IOrder order , HttpClient httpclient
                              , ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {

        //Get Product from Anuther Services
        public async Task<ProductDTO> GetProduct(int ProductId)
        {
            //call product Api using httpClint
            var result = await httpclient.GetAsync($"/api/Products/{ProductId}");

            if (!result.IsSuccessStatusCode)
                return null!;

            var productDTO = await result.Content.ReadFromJsonAsync<ProductDTO>();
            return productDTO!;
        }

        //GetUser
        public async Task<AppUserDTO> GetUser(int Userid)
        {
            //call product Api using httpClint
            var result = await httpclient.GetAsync($"https://localhost:22628/api/Authentication/{Userid}");

            if (!result.IsSuccessStatusCode)
                return null!;

            var UserDTO = await result.Content.ReadFromJsonAsync<AppUserDTO>();
            return UserDTO!;
        }





        public async Task<OrderDetailsDTO> GetOrderDetailsByOrderId(int orderId)
        {
            var _order = await order.FindByIdAsync(orderId);

            if( _order is null || _order.Id<=0)
                return null!;

            //retry pipeline to get _order
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            //Prepare Product
            var productDTO = await retryPipeline.ExecuteAsync( async token => await GetProduct(_order.ProductId));

            //Prepare User
            var userDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(_order.ClientId));


            return new OrderDetailsDTO(_order.Id,
                productDTO.id,
                _order.ClientId,
                userDTO.Name,
                _order.PurchaseQuntity,
                userDTO.Email,
                userDTO.Address,
                userDTO.TelephoneNumber,
                productDTO.Name,
                 productDTO.Price,
                 productDTO.Price * _order.PurchaseQuntity,
                 _order.OrderDate

                );

        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
               

            var orders = await order.GetOrdersAsync(o => o.ClientId == clientId);

            if (!orders.Any()) return Enumerable.Empty<OrderDTO>();

            var result = OrderConversion.FromEntity(null, orders);

            return result.Item2!;

        }
    }
}
