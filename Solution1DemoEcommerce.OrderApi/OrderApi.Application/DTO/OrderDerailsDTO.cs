using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTO
{
    public record OrderDerailsDTO(
       [Required] int OrderID,
       [Required] int ProductId,
       [Required] int ClientId,
       [Required] int PurchaseQuantity,
       [Required , EmailAddress]string Email,
       [Required] string TelephoneNumber,
       [Required] string ProductName,
       [Required , DataType(DataType.Currency)]decimal UnitPrice,
       [Required, DataType(DataType.Currency)] decimal TotalPrice,
       [Required] DateTime OrderDate);
}



