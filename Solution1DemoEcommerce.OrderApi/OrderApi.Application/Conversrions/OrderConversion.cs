using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderApi.Application.DTO;
using OrderApi.Domin.Entites;

namespace OrderApi.Application.Conversrions;

public static class OrderConversion
{
    public static Order ToEntity(OrderDTO dTO)
    {
        if (dTO == null)
            return new();

        return new Order
        {
            Id = dTO.ID,
            ClientId = dTO.ClientId,
            OrderDate = dTO.OrderDate,
            ProductId = dTO.ProductId,
            PurchaseQuntity = dTO.ProductId
        };

    }
    public static  (OrderDTO?,IEnumerable<OrderDTO>?) FromEntity(Order? order ,IEnumerable<Order>? orders)
    {
        if (orders is null || order is not null)
            return (new OrderDTO(order!.Id, order.ProductId,
                 order.ClientId, order.PurchaseQuntity
                  , order.OrderDate), null);

        if (order is null  || orders is not null)
            return (null,orders!.Select(o => new OrderDTO(o.Id, o.ProductId, o.ClientId,
                o.PurchaseQuntity, o.OrderDate)));


        return (null, null);
    }



}
