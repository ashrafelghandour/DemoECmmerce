using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApi.Application.DTO
{
    public record OrderDTO(int ID ,
       [Required,Range(1,int.MaxValue)]int ProductId,
        [Required, Range(1, int.MaxValue)] int ClientId,
         [Required, Range(1, int.MaxValue)] int PurchaseQuantity,
         DateTime OrderDate);
}



