using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using ProductApi.Domain.Entities;
using eCommerce.SharedLibrary.Responses;

namespace ProductApi.Tests.Repositories
{
    public class ProductRepositoryTests
    {
        private readonly ProductDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductTestDb_Best")
                .Options;

            _context = new ProductDbContext(options);
            _repository = new ProductRepository(_context);
        }

        private Product CreateSampleProduct(string name = "TestProduct") =>
            new Product
            {
                Name = name,
                Description = "Sample Description",
                Quantity = 10,
                Price = 50.5m
            };

        [Fact]
        public async Task CreateAsync_WhenProductIsNull_ShouldReturnErrorResponse()
        {
            // Act
            var result = await _repository.CteateAsync(null);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Error new product is null");
        }

        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExists_ShouldReturnDuplicateError()
        {
            // Arrange
            var product = CreateSampleProduct();
            _context.products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var duplicate = CreateSampleProduct(); // same name
            var result = await _repository.CteateAsync(duplicate);

            // Assert
            result.Flag.Should().BeFalse();
            result.Message.Should().Contain("already added");
        }

        [Fact]
        public async Task CreateAsync_WhenProductIsValid_ShouldAddSuccessfully()
        {
            // Arrange
            var product = CreateSampleProduct("UniqueProduct");

            // Act
            var result = await _repository.CteateAsync(product);

            // Assert
            result.Flag.Should().BeTrue();
            _context.products.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _repository.FindByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductExists_ShouldReturnProduct()
        {
            // Arrange
            var product = CreateSampleProduct("FindProduct");
            _context.products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.FindByIdAsync(product.Id);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("FindProduct");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductExists_ShouldUpdateSuccessfully()
        {
            // Arrange
            var product = CreateSampleProduct("OldName");
            _context.products.Add(product);
            await _context.SaveChangesAsync();

            product.Name = "UpdatedName";

            // Act
            var result = await _repository.UpdateAsync(product);

            // Assert
            result.Flag.Should().BeTrue();
            result.Message.Should().Contain("Updated");
        }

        [Fact]
        public async Task UpdateAsync_WhenProductNotExists_ShouldReturnNotFound()
        {
            // Arrange
            var product = CreateSampleProduct("NonExistent");
            product.Id = 999; // not in DB

            // Act
            var result = await _repository.UpdateAsync(product);

            // Assert
            result.Flag.Should().BeFalse();
            result.Message.Should().Contain("not found");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductExists_ShouldRemoveSuccessfully()
        {
            // Arrange
            var product = CreateSampleProduct("DeleteMe");
            _context.products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(product);

            // Assert
            result.Flag.Should().BeTrue();
            _context.products.Count().Should().Be(0);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductNotExists_ShouldReturnError()
        {
            // Arrange
            var product = CreateSampleProduct("Ghost");
            product.Id = 999; // not in DB

            // Act
            var result = await _repository.DeleteAsync(product);

            // Assert
            result.Flag.Should().BeFalse();
            result.Message.Should().Contain("not Exesit");
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ShouldReturnListOfProducts()
        {
            // Arrange
            _context.products.Add(CreateSampleProduct("P1"));
            _context.products.Add(CreateSampleProduct("P2"));
            await _context.SaveChangesAsync();

            // Act
            var products = await _repository.GetAllAsync();

            // Assert
            products.Should().NotBeNull();
            products.Count().Should().BeGreaterThan(1);
        }
    }
}
