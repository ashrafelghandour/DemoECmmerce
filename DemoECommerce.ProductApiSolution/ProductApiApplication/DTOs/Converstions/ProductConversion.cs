using ProductApi.Domain.Entities;

namespace ProductApiApplication.DTOs.ProductConverstion
{
  
    public static class ProductConversion 
    {
        public static Product ToEntitiy(ProductDTO product) => new Product
        {
            Id = product.id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        };
        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEintity(Product? product, IEnumerable<Product>? Products)
        {
            if (product is not null || Products is null)
            {
                return (new ProductDTO(
                
                     product!.Id,
                     product.Name!,
                     product.Description,
                     product.Quantity,
                     product.Price

                ), null);
            }

            if (product is null || Products is not null)
            {
                return (null
                 , Products.Select(pro => new ProductDTO(
                 
                    pro!.Id ,
                    pro.Name!,
                    pro.Description,
                    pro.Quantity,
                    pro.Price
                 )));
            }
            else
            {
                return (null,null);
            }
        }

      
    }
}
