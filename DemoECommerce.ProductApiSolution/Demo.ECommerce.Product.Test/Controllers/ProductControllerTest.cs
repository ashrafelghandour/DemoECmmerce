using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Presentaion.Controllers;
using ProductApiApplication.DTOs.ProductConverstion;
using ProductApiApplication.DTOs;
using ProductApiApplication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using eCommerce.SharedLibrary.Responses;
using Castle.Components.DictionaryAdapter.Xml;

namespace Demo.ECommerce.Product.Test.Controllers;

public class ProductControllerTest
{
    private readonly IProduct productInterFace;
    private readonly ProductsController controller;

    public ProductControllerTest()
    {
        //Set up dependenise
       productInterFace = A.Fake<IProduct>();
       //set up system under test
       controller = new ProductsController(productInterFace);
        
    }
   
    [Fact]
    public async Task GetProducts_WhenProudctExists_ShouldReturnOkWithProductsList()
    {
        //Arrange
        var products =new List<ProductApi.Domain.Entities.Product>()
        {
            new(){Id = 1 , Name ="Product 1", Description = " fsda", Price = 100.22m, Quantity =500},
            new(){Id = 2 , Name ="Product 2", Description = " ffsdsda", Price = 10.22m, Quantity =300}
        }; 
        //set up fake respoanse
        A.CallTo(()=> productInterFace.GetAllAsync()).Returns(products);

        //act
        var result = await controller.GetProducts();

        var okResult = result.Result as OkObjectResult;

        var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;

        //assert


        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);
        Assert.True(returnedProducts?.Count() == 2);
    }


    [Fact]
    public async Task GetProducts_WhenDataToBeNll_ShouldReturnNotFoundWithMessage()
    {
        //Arrange
       
        //set up fake respoanse
        A.CallTo(() => productInterFace.GetAllAsync()).Returns(new List<ProductApi.Domain.Entities.Product>());

        //act
        var result = await controller.GetProducts();


        //assert
        var NotFoundResult = result.Result as NotFoundObjectResult;
        //okResult.Should().NotBeNull();
        //okResult.Should().Be(StatusCodes.Status200OK);

       // var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
        //returnedProducts.Should().NotBeNull();
        //returnedProducts.Should().HaveCount(2);
        //returnedProducts.First().id.Should().Be(1);
        //returnedProducts.Last().id.Should().Be(2);

        
        Assert.Equal(StatusCodes.Status404NotFound, NotFoundResult?.StatusCode);
        var message = NotFoundResult.Value as string;
        message.Should().Be("No products in database");
    }

    //[Fact]
    //public async Task GetProducts_WhenConversionReturnsEmpty_ShouldReturnNotFound()
    //{
    //    // Arrange
    //    var products = new List<ProductApi.Domain.Entities.Product>
    //    {
    //       new() { Id = 1, Name = "Test", Description = "Desc", Price = 10, Quantity = 5 }
    //    };

    //    A.CallTo(() => productInterFace.GetAllAsync()).Returns(products);


    //    A.CallTo(() => ProductConversion.FromEintity(null, products).Item2)
    //        .Returns(new List<ProductDTO>());

    //    // Act
    //    var result = await controller.GetProducts();

    //    // Assert
    //    var notFoundResult = result.Result as NotFoundObjectResult;
    //    Assert.NotNull(notFoundResult);
    //    Assert.Equal(StatusCodes.Status404NotFound, notFoundResult?.StatusCode);
    //    Assert.Equal("Not found Products", notFoundResult?.Value);
    //}



    [Fact]
    public async Task CreateProduct_WhenModelStateIsInvalid_SheludReturnBadeRequstWithModelState()
    {
        //Arrange
        var newproduct = new ProductDTO(1,"","", 50, 12.4m);
        controller.ModelState.AddModelError("Name","Required");
        //Act
         
        var sut = await controller.CreateProduct(newproduct);

        //Assert

        var result = sut.Result as BadRequestObjectResult;
        result.Should().NotBeNull();
        Assert.True(StatusCodes.Status400BadRequest == result.StatusCode);
        
    }
    [Fact]
    public async Task CreateProduct_WhenMethodCreatAsyncReturnRespnseWithFalse_SheludReturnBadeRequstWithResponse()
    {
        //Arrange
        var newproduct = new ProductDTO(1, "test1", "www", 50, 12.4m);
        var product = ProductConversion.ToEntitiy(newproduct);
        A.CallTo( ()=>  productInterFace.CteateAsync(product)).
            Returns(new Response(false,"err"));
        //Act

        var sut = await controller.CreateProduct(newproduct);

        //Assert

        var result = sut.Result as BadRequestObjectResult;
        result.Should().NotBeNull();
        Assert.True(StatusCodes.Status400BadRequest == result.StatusCode);

    }
   

    [Fact]
    public async Task CreateProduct_WhenProductCreated_ShouldReturnOkWithResponse()
    {
        // Arrange
        var newproduct = new ProductDTO(1, "test1", "www", 50, 12.4m);
        var response = new Response(true, "Successfully");

        A.CallTo(() => productInterFace.CteateAsync(A<ProductApi.Domain.Entities.Product>.Ignored))
            .Returns(response);

        // Act
        var sut = await controller.CreateProduct(newproduct);

        // Assert
        var result = sut.Result as OkObjectResult;
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);

        var returnedResponse = result.Value as Response;
        Assert.NotNull(returnedResponse);
        Assert.True(returnedResponse.Flag);
        Assert.Equal("Successfully", returnedResponse.Message);
    }


   
  

    [Fact]
    public async Task UpdateProduct_WhenTheStateModelIsInvalid_ShouldReturnBadRequstWithModelState()
    {
        //Arrange
        var newproduct = new ProductDTO(1, "", "", 50, 12.4m);
        controller.ModelState.AddModelError("Name", "Required");

        //Act
        var sut = await controller.UpdateProduct(newproduct);
         
        //Assert

        var BadResult = sut.Result as BadRequestObjectResult;
        BadResult.Should().NotBeNull();
        BadResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);  
    }

    [Fact]
    public async Task UpdateProduct_WhenUpdateMethodTeturnRespaoneWithFasle_ShouldReturnBadRequstWithRespone()
    {
        //Arrange
        var newproduct = new ProductDTO(1, "", "", 50, 12.4m);
        A.CallTo(() => productInterFace.UpdateAsync(A<ProductApi.Domain.Entities.Product>.Ignored)).
                Returns(new Response(false, "Failed"));
        //Act
        var sut = await controller.UpdateProduct(newproduct);

        //Assert

        var BadResult = sut.Result as BadRequestObjectResult;
        BadResult.Should().NotBeNull();
        BadResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    
    
        var respone = BadResult.Value as Response;
        respone.Should().NotBeNull();
        respone.Flag!.Should().BeFalse();
        respone.Message!.Should().Be("Failed");
    }

    [Fact]
    public async Task UpdateProduct_UpdatedSuccessfuly_ShouldReturnOkWithRespone()
    {
        //Arrange
        var newproduct = new ProductDTO(1, "", "", 50, 12.4m);
        A.CallTo(() => productInterFace.UpdateAsync(A<ProductApi.Domain.Entities.Product>.Ignored)).
                Returns(new Response(true, "Updated"));
        //Act
        var sut = await controller.UpdateProduct(newproduct);

        //Assert

        var OkResult = sut.Result as OkObjectResult;
        OkResult.Should().NotBeNull();
        OkResult.StatusCode.Should().Be(StatusCodes.Status200OK);


        var respone = OkResult.Value as Response;
        respone.Should().NotBeNull();
        respone.Flag!.Should().BeTrue();
        respone.Message!.Should().Be("Updated");
    }

   
    [Fact]
    public async Task DeleteProduct_WhenTheStateModelIsInvalid_ShouldReturnBadRequstWithModelState()
    {
        //Arrange
        var newproduct = new ProductDTO(1, "", "", 50, 12.4m);
        controller.ModelState.AddModelError("ID", "Required");

        //Act
        var sut = await controller.DeleteProduct(newproduct);

        //Assert

        var BadResult = sut.Result as BadRequestObjectResult;
        BadResult.Should().NotBeNull();
        BadResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task DeleteProduct_WhenDeleteMethodTeturnRespaoneWithFasle_ShouldReturnBadRequstWithRespone()
    {
        //Arrange
        var newproduct = new ProductDTO(1, "", "", 50, 12.4m);
        A.CallTo(() => productInterFace.DeleteAsync(A<ProductApi.Domain.Entities.Product>.Ignored)).
                Returns(new Response(false, "Failed"));
        //Act
        var sut = await controller.DeleteProduct(newproduct);

        //Assert

        var BadResult = sut.Result as BadRequestObjectResult;
        BadResult.Should().NotBeNull();
        BadResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);


        var respone = BadResult.Value as Response;
        respone.Should().NotBeNull();
        respone.Flag!.Should().BeFalse();
        respone.Message!.Should().Be("Failed");
    }

    [Fact]
    public async Task DeleteProduct_WhemDeleteSuccessfuly_ShouldReturnOkWithRespone()
    {
        //Arrange
        var newproduct = new ProductDTO(1, "", "", 50, 12.4m);
        A.CallTo(() => productInterFace.DeleteAsync(A<ProductApi.Domain.Entities.Product>.Ignored)).
                Returns(new Response(true, "Delete"));
        //Act
        var sut = await controller.DeleteProduct(newproduct);

        //Assert

        var OkResult = sut.Result as OkObjectResult;
        OkResult.Should().NotBeNull();
        OkResult.StatusCode.Should().Be(StatusCodes.Status200OK);


        var respone = OkResult.Value as Response;
        respone.Should().NotBeNull();
        respone.Flag!.Should().BeTrue();
        respone.Message!.Should().Be("Delete");
    }

    [Fact]
    public async Task GetProduct_WhenFindByIdAsyncToReturnNull_ShouldRetusbNotFound()
    {
        //Arrange
        A.CallTo(() => productInterFace.FindByIdAsync(A<int>.Ignored))
        .Returns((ProductApi.Domain.Entities.Product?)null);
        //Act
        var  sut = await controller.GetProduct(0);
          var notFoundResult =  sut.Result as NotFoundObjectResult;
        //Assert
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var message = notFoundResult.Value as string;
        message.Should().Be("not found product with id 0");
    }
    [Fact]
    public async Task GetProduct_WhenFoundProduct_ShouldReturnOk()
    {
        // Arrange
        var fakeProduct = new ProductApi.Domain.Entities.Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Sample",
            Quantity = 10,
            Price = 15.5m
        };

        A.CallTo(() => productInterFace.FindByIdAsync(A<int>.Ignored))
            .Returns(fakeProduct);

        // Act
        var sut = await controller.GetProduct(1);

        // Assert
        var okResult = sut.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var productDto = okResult.Value as ProductDTO;
        productDto.Should().NotBeNull();
        productDto.id.Should().Be(1);
    }

}

