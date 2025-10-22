using System.Linq.Expressions;
using eCommerce.SharedLibrary.Interfaces;
using OrderApi.Domin.Entites;

namespace OrderApi.Application.Interfaces
{
    public interface IOrder : IGenaricInterface<Order>
    {
        Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order,bool>> predicate);
    }
}
