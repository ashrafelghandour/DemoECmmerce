using OrderApi.Application.DTO;

namespace OrderApi.Application.Serveces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId);
        Task<OrderDetailsDTO> GetOrderDetailsByOrderId(int orderId);
    }
}
