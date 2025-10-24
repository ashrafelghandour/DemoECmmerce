using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTO
{
    public record OrderDetailsDTO(
       [Required] int OrderID,
       [Required] int ProductId,
       [Required] int ClientId,
       [Required] string ClientName,
       [Required] int PurchaseQuantity,
       [Required, EmailAddress]string Email,
       [Required] string Address,
       [Required] string TelephoneNumber,
       [Required] string ProductName,
       [Required , DataType(DataType.Currency)]decimal UnitPrice,
       [Required, DataType(DataType.Currency)] decimal TotalPrice,
       [Required] DateTime OrderDate);
}



