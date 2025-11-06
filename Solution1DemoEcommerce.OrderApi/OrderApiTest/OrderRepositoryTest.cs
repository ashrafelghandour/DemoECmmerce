using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTO;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Serveces;
using OrderApi.Application.Conversrions;
using OrderApi.Domin.Entites;
using Polly;
using Polly.Registry;
using Xunit;
using System.Linq.Expressions;

public class OrderServiceTests
{
    private readonly IOrder _fakeOrderRepo;
    private readonly ResiliencePipelineProvider<string> _fakePipelineProvider;
    private readonly HttpClient _fakeHttpClient;

    public OrderServiceTests()
    {
        _fakeOrderRepo = A.Fake<IOrder>();
        _fakePipelineProvider = A.Fake<ResiliencePipelineProvider<string>>();
        A.CallTo(() => _fakePipelineProvider.GetPipeline(A<string>.Ignored))
            .Returns(ResiliencePipeline.Empty);

        var fakeHandler = new FakeHttpMessageHandler();
        _fakeHttpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("http://localhost")
        };
    }

    // ----------------------------- //
    //          GetProduct           //
    // ----------------------------- //
    [Fact]
    public async Task GetProduct_ShouldReturn_ProductDTO_WhenApiReturnsOk()
    {
        // Arrange
        var product = new ProductDTO(10,"Keyboard","FSDAf",2,250m );
        FakeHttpMessageHandler.FakeResponses = new()
        {
            ["/api/Products/10"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(product)
            }
        };

        var service = new OrderService(_fakeOrderRepo, _fakeHttpClient, _fakePipelineProvider);

        // Act
        var result = await service.GetProduct(10);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Keyboard");
    }

    [Fact]
    public async Task GetProduct_ShouldReturnNull_WhenApiFails()
    {
        // Arrange
        FakeHttpMessageHandler.FakeResponses = new()
        {
            ["/api/Products/10"] = new HttpResponseMessage(HttpStatusCode.BadRequest)
        };

        var service = new OrderService(_fakeOrderRepo, _fakeHttpClient, _fakePipelineProvider);

        // Act
        var result = await service.GetProduct(10);

        // Assert
        result.Should().BeNull();
    }

    // ----------------------------- //
    //            GetUser            //
    // ----------------------------- //
    [Fact]
    public async Task GetUser_ShouldReturn_UserDTO_WhenApiReturnsOk()
    {
        // Arrange
        var user = new AppUserDTO ("Ashraf","01201220","fsdafasd","ashraf@test.com","SDFASDF","admin");
        FakeHttpMessageHandler.FakeResponses = new()
        {
            ["/api/Authentication/1"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(user)
            }
        };

        var service = new OrderService(_fakeOrderRepo, _fakeHttpClient, _fakePipelineProvider);

        // Act
        var result = await service.GetUser(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Ashraf");
    }

    [Fact]
    public async Task GetUser_ShouldReturnNull_WhenApiFails()
    {
        // Arrange
        FakeHttpMessageHandler.FakeResponses = new()
        {
            ["api/Authentication/1"] = new HttpResponseMessage(HttpStatusCode.NotFound)
        };

        var service = new OrderService(_fakeOrderRepo, _fakeHttpClient, _fakePipelineProvider);

        // Act
        var result = await service.GetUser(1);

        // Assert
        result.Should().BeNull();
    }

    // ----------------------------- //
    //    GetOrderDetailsByOrderId   //
    // ----------------------------- //
    [Fact]
    public async Task GetOrderDetailsByOrderId_ShouldReturnDetails_WhenDataIsValid()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            ClientId = 100,
            ProductId = 200,
            PurchaseQuntity = 2,
            OrderDate = DateTime.Now
        };

        A.CallTo(() => _fakeOrderRepo.FindByIdAsync(order.Id))
            .Returns(order);

        var product =   new ProductDTO(10, "Laptop", "FSDAf", 2, 250m);
        var user = new AppUserDTO("Ashraf", "01201220", "fsdafasd", "ashraf@test.com", "SDFASDF", "admin");

        FakeHttpMessageHandler.FakeResponses = new()
        {
            ["/api/Products/200"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(product)
            },
            ["/api/Authentication/100"] = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(user)
            }
        };

        var service = new OrderService(_fakeOrderRepo, _fakeHttpClient, _fakePipelineProvider);

        // Act
        var result = await service.GetOrderDetailsByOrderId(1);

        // Assert
        result.Should().NotBeNull();
        result.ProductName.Should().Be("Laptop");
        result.ClientName.Should().Be("Ashraf");
    }

    [Fact]
    public async Task GetOrderDetailsByOrderId_ShouldReturnNull_WhenOrderNotFound()
    {
        // Arrange
        A.CallTo(() => _fakeOrderRepo.FindByIdAsync(999)).Returns((Order)null!);

        var service = new OrderService(_fakeOrderRepo, _fakeHttpClient, _fakePipelineProvider);

        // Act
        var result = await service.GetOrderDetailsByOrderId(999);

        // Assert
        result.Should().BeNull();
    }

    // ----------------------------- //
    //       GetOrdersByClientId     //
    // ----------------------------- //
    [Fact]
    public async Task GetOrdersByClientId_ShouldReturnOrders_WhenFound()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order { Id = 1, ClientId = 10, ProductId = 5, PurchaseQuntity = 3, OrderDate = DateTime.Now },
            new Order { Id = 2, ClientId = 10, ProductId = 8, PurchaseQuntity = 1, OrderDate = DateTime.Now }
        };

        A.CallTo(() => _fakeOrderRepo.GetOrdersAsync(A<Expression< Func<Order, bool>>>._))
            .Returns(orders);

        var service = new OrderService(_fakeOrderRepo, _fakeHttpClient, _fakePipelineProvider);

        // Act
        var result = await service.GetOrdersByClientId(10);

        // Assert
        result.Should().NotBeEmpty();
        result.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetOrdersByClientId_ShouldReturnEmpty_WhenNoOrders()
    {
        // Arrange
        A.CallTo(() => _fakeOrderRepo.GetOrdersAsync(A<Expression<Func<Order, bool>>>._))
            .Returns(new List<Order>());

        var service = new OrderService(_fakeOrderRepo, _fakeHttpClient, _fakePipelineProvider);

        // Act
        var result = await service.GetOrdersByClientId(999);

        // Assert
        result.Should().BeEmpty();
    }
}

/// <summary>
/// Fake Http Handler لتزييف الردود الخاصة بـ HttpClient
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    public static Dictionary<string, HttpResponseMessage> FakeResponses { get; set; } = new();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (FakeResponses.TryGetValue(request.RequestUri!.PathAndQuery, out var response))
        {
            return Task.FromResult(response);
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}
