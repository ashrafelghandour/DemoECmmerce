using System.Linq.Expressions;
using eCommerce.SharedLibrary;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OrderApi.Application.DTO;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Serveces;
using OrderApi.Domin.Entites;
using OrderApi.Infrastructure.Data;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext orderDb) : IOrder
    {
        public async Task<Response> CteateAsync(Domin.Entites.Order entity)
        {
            try
            {
              var order = await orderDb.orders.AddAsync(entity);
                await orderDb.SaveChangesAsync();
                return order is not null && order.Entity.Id > 0? new Response(true, "Order placed successfully") 
                              : new Response(false, "Error occurred while placing order");
            }
            catch (Exception ex) {

                LogException.LogExceptions(ex);

                return new Response(false,  ex.Message);
            }
        }

        public async Task<Response> DeleteAsync(Domin.Entites.Order entity)
        {
            try
            {
                var isFount = await orderDb.orders.AnyAsync(o => o.Id == entity.Id);
                if (!isFount)
                    return new Response(false, "This Order is not Excist");

                orderDb.orders.Remove(entity);

                if (orderDb.SaveChanges()<1)
                {
                    return new Response(false, "Error occurred while Deleted order");

                }

                return new Response(true, "Order Deleted Successfully");

            }

            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                return new Response(false, ex.Message);
            }
        }

        public async Task<Domin.Entites.Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await orderDb.orders.FindAsync(id);
                return order is not null ? order : null!;

            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                throw new Exception(ex.Message);

            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await orderDb.orders.AsNoTracking().ToListAsync();
                return orders is null? null!:orders;
            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                throw new Exception(ex.Message);

            }
        }

        public async Task<Domin.Entites.Order> GetByAsync(Expression<Func<Domin.Entites.Order, bool>> predicate)
        {
            try
            {
                var order = await orderDb.orders.Where(predicate).FirstOrDefaultAsync();


                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                throw new Exception(ex.Message);

            }
        }

        

        public async Task<IEnumerable<Domin.Entites.Order>> GetOrdersAsync(Expression<Func<Domin.Entites.Order, bool>> predicate)
        {
            try
            {

                return await orderDb.orders
                    .AsNoTracking()
                    .Where(predicate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw; // لا تنشئ Exception جديد — يرمي الأصلي
            }
        }

        public async Task<IEnumerable<Domin.Entites.Order>> GetOrdersByCleintIdAsync(int id)
        {
            try
            {


                      return await orderDb.orders
                       .Where(o=>o.ClientId == id)
                           .ToListAsync();
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw; // لا تنشئ Exception جديد — يرمي الأصلي
            }
        }


        public async Task<Response> UpdateAsync(Domin.Entites.Order entity)
        {
            try
            {
                var orderOld = await FindByIdAsync(entity.Id);
                if(orderOld is null || orderOld.Id <1){
                    return new Response(false, "The order is not excist");
                }

                orderDb.Entry(orderOld).State = EntityState.Detached;
                orderDb.orders.Update(entity);
                await orderDb.SaveChangesAsync();
                return new Response(true,"order updated ");
            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                return new Response(false, ex.Message);
            }
        }
    }
}
