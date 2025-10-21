
using System.Linq.Expressions;
using eCommerce.SharedLibrary.Responses;

namespace eCommerce.SharedLibrary.Interfaces
{
    public  interface IGenaricInterface <T> where T : class
    {
        Task<Response> CteateAsync(T entity);
        Task<Response> UpdateAsync(T entity);
        Task<Response> DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> FindByIdAsync(int id);

        Task<T> GetByAsync(Expression<Func<T, bool>> predicate); 

    }
}
