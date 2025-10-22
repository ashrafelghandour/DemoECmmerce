using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTO
{
    public record ProductDTO
       (

       int id,
       [Required] string Name,
        string? Description,
        [Required, Range(1,int.MaxValue)]
         int Quantity,
        [Required,DataType(DataType.Currency)]
         decimal Price

   );
}



