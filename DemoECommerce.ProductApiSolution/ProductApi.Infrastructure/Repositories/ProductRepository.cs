using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eCommerce.SharedLibrary;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApiApplication.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext db) : IProduct
    {
        public async Task<Response> CteateAsync(Product entity)
        {
            try
            {
                //check if the product already exist

                if (entity is  null) 
                    return new Response(false, "Error new product is null");

                var getprocut = await GetByAsync(p => p.Name!.Equals(entity!.Name));
                
                if(getprocut is not null && !string.IsNullOrEmpty(getprocut.Name))
                    return new Response(false, $"{entity.Name} already added."); 
                
                var currentEntity = db.products.Add(entity).Entity;
                await db.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} added to databse successfully");
                else
                {
                    return new Response(false, $"Error occured while adding {entity.Name}");
                }
               

            }
            catch(Exception ex) {
                // log the original exeption
                LogException.LogExceptions(ex);

                //display scary free message to the clint
                return new Response(false, "Error occurrc adding new product");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {

                var product = await db.products.FindAsync(entity);

                if (product is null)
                    return new Response(false, $"this product is not Exesit{entity.Name}");
                else
                    db.products.Remove(product);
                    var rec = await db.SaveChangesAsync();
                if (rec > 0)
                    return new Response(true, $"this product {entity.Name} Deleted successflly");


                return new Response(false, "Error occurrc while deleting product");


            }
            catch (Exception ex)
            {
                // log the original exeption
                LogException.LogExceptions(ex);

                //display scary free message to the clint
                return new Response(false, "Error occurrc deleting  product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {


                var product =await  db.products.FindAsync(id);
                return product is not null? product : null;

            }
            catch (Exception ex)
            {
                // log the original exeption
                LogException.LogExceptions(ex);

                //display scary free message to the clint
                throw new Exception( "Error occurrc retrieving product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await db.products.ToListAsync();
                return products != null ? products : null; ;

            }
            catch (Exception ex)
            {
                // log the original exeption
                LogException.LogExceptions(ex);

                //display scary free message to the clint
                throw new Exception("Error occurrc retrieving products");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
           try
           {


                    var product = await db.products.SingleOrDefaultAsync(predicate);
                    return product is not null ? product : null;

           }
           catch (Exception ex)
           {
                    // log the original exeption
                    LogException.LogExceptions(ex);

                    //display scary free message to the clint
                    throw new Exception("Error occurrc retrieving product");
           }

           
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {

                var product = await db.products.FindAsync(entity.Id);
                if (product == null)
                    return  new Response(false, $"{entity.Name} not found"); 

                db.Entry(product).State = EntityState.Detached;
                db.products.Update(entity);
                await db.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is Updated Successfully"); 
            }
            catch (Exception ex)
            {
                // log the original exeption
                LogException.LogExceptions(ex);

                //display scary free message to the clint
                throw new Exception("Error occurrc retrieving product");
            }

        }
    }
}
