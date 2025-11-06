using Xunit;
using FluentAssertions;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using eCommerce.SharedLibrary.Responses;
using OrderApi.Application.DTO;
using OrderApi.Application.Interfaces;
using OrderApi.Perisntation.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderApi.Application.Serveces;
using OrderApi.Domin.Entites;

namespace OrderApi.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly IOrder _fakeOrder;
        private readonly IOrderService _fakeOrderService;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _fakeOrder = A.Fake<IOrder>();
            _fakeOrderService = A.Fake<IOrderService>();
            _controller = new OrdersController(_fakeOrder, _fakeOrderService);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnOk_WhenOrdersExist()
        {
            // Arrange
            var orders = new List<Domin.Entites.Order> { new() { Id = 1 }, new() { Id = 2 } };
            A.CallTo(() => _fakeOrder.GetAllAsync()).Returns(orders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeAssignableTo<IEnumerable<Order>>()
                .Which.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrders_ShouldReturnNotFound_WhenNoOrders()
        {
            // Arrange
            A.CallTo(() => _fakeOrder.GetAllAsync()).Returns(new List<Order>());

            // Act
            var result = await _controller.GetOrders();

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("No Orders in database");
        }

        [Fact]
        public async Task GetOrder_ShouldReturnOk_WhenOrderExists()
        {
            var dto = new Order { Id = 5, ClientId = 10 };
            A.CallTo(() => _fakeOrder.FindByIdAsync(5)).Returns(dto);

            var result = await _controller.GetOrder(5);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.Value.Should().BeOfType<Application.DTO.OrderDTO>();
        }

        [Fact]
        public async Task GetOrder_ShouldReturnNotFound_WhenOrderMissing()
        {
            A.CallTo(() => _fakeOrder.FindByIdAsync(1)).Returns((Order?)null);

            var result = await _controller.GetOrder(1);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnOk_WhenResponseFlagTrue()
        {
            var dto = new OrderDTO(1,2,3,4,DateTime.Now);
            A.CallTo(() => _fakeOrder.CteateAsync(A<Order>.Ignored))
                .Returns(new Response { Flag = true, Message = "Created" });

            var result = await _controller.CreateProduct(dto);

            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<Response>()
                .Which.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProduct_ShouldReturnBadRequest_WhenResponseFlagFalse()
        {
            var dto = new OrderDTO(1, 2, 3, 4, DateTime.Now);
            A.CallTo(() => _fakeOrder.CteateAsync(A<Order>.Ignored))
                .Returns(new Response { Flag = false, Message = "Error" });

            var result = await _controller.CreateProduct(dto);

            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeOfType<Response>()
                .Which.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnOk_WhenFlagTrue()
        {
            var dto = new OrderDTO(1, 2, 3, 4, DateTime.Now);
            A.CallTo(() => _fakeOrder.UpdateAsync(A<Order>.Ignored))
                .Returns(new Response { Flag = true });

            var result = await _controller.UpdateProduct(dto);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateProduct_ShouldReturnBadRequest_WhenFlagFalse()
        {
            var dto = new OrderDTO(1, 2, 3, 4, DateTime.Now);
            A.CallTo(() => _fakeOrder.UpdateAsync(A<Order>.Ignored))
                .Returns(new Response { Flag = false });

            var result = await _controller.UpdateProduct(dto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnOk_WhenFlagTrue()
        {
            var dto = new OrderDTO(1, 2, 3, 4, DateTime.Now);
            A.CallTo(() => _fakeOrder.DeleteAsync(A<Order>.Ignored))
                .Returns(new Response { Flag = true });

            var result = await _controller.DeleteProduct(dto);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnBadRequest_WhenFlagFalse()
        {
            var dto = new OrderDTO(1, 2, 3, 4, DateTime.Now);
            A.CallTo(() => _fakeOrder.DeleteAsync(A<Order>.Ignored))
                .Returns(new Response { Flag = false });

            var result = await _controller.DeleteProduct(dto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetClientOrder_ShouldReturnBadRequest_WhenClientIdInvalid()
        {
            var result = await _controller.GetClientOrder(0);

            result.Result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Invalid data provided");
        }

        [Fact]
        public async Task GetClientOrder_ShouldReturnOk_WhenOrdersFound()
        {
            var orders = new List<OrderDTO> { new OrderDTO(1, 2, 3, 4, DateTime.Now) };
            A.CallTo(() => _fakeOrderService.GetOrdersByClientId(5)).Returns(orders);

            var result = await _controller.GetClientOrder(5);

            var ok = result.Result.Should().BeOfType<OkObjectResult>();
             ;
        }

        [Fact]
        public async Task GetClientOrderV2_ShouldReturnBadRequest_WhenInvalidId()
        {
            var result = await _controller.GetClientOrderV2(0);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetClientOrderV2_ShouldReturnOk_WhenOrdersFound()
        {
            var list = new List<Order> { new Order{Id =1,ClientId=2 }  };
            A.CallTo(() => _fakeOrder.GetAllAsync()).Returns(list);

            var result = await _controller.GetClientOrderV2(10);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IEnumerable<OrderDTO>>();
        }

        [Fact]
        public async Task GetOrderDetails_ShouldReturnBadRequest_WhenInvalidId()
        {
            var result = await _controller.GetOrderDetails(0);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetOrderDetails_ShouldReturnOk_WhenFound()
        {
            var details = new OrderDetailsDTO(5, 2, 3,"fdas",3,"sa0","asf","a","as",44m,44m,DateTime.Now);
            A.CallTo(() => _fakeOrderService.GetOrderDetailsByOrderId(5))
                .Returns(details);

            var result = await _controller.GetOrderDetails(5);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeOfType<OrderDetailsDTO>()
                .Which.OrderID.Should().Be(5);
        }

        [Fact]
        public async Task GetOrderDetails_ShouldReturnNotFound_WhenNotFound()
        {
            A.CallTo(() => _fakeOrderService.GetOrderDetailsByOrderId(9))
                .Returns( new OrderDetailsDTO(0, 2, 3, "fdas", 3, "sa0", "asf", "a", "as", 44m, 44m, DateTime.Now)
);

            var result = await _controller.GetOrderDetails(9);

            result.Result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be("Not order founf");
        }
    }
}
