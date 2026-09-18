using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _service = new ProductService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ProductExists_ReturnsProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Laptop",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            Assert.Equal("Laptop", result.ProductName);

            _repositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ProductDoesNotExist_ReturnsNull()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);

            _repositoryMock.Verify(
                x => x.GetByIdAsync(999),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var dto = new CreateProductDto
            {
                ProductName = "Keyboard"
            };

            var createdProduct = new Product
            {
                Id = 2,
                ProductName = "Keyboard",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync(createdProduct);

            // Act
            var result = await _service.CreateAsync(dto, "admin");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("Keyboard", result.ProductName);
            Assert.Equal("admin", result.CreatedBy);

            _repositoryMock.Verify(
                x => x.AddAsync(It.Is<Product>(p =>
                    p.ProductName == "Keyboard" &&
                    p.CreatedBy == "admin")),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ProductExists_ReturnsTrue()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Old Name",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow
            };

            var dto = new UpdateProductDto
            {
                ProductName = "New Name"
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(
                1,
                dto,
                "manager");

            // Assert
            Assert.True(result);
            Assert.Equal("New Name", product.ProductName);
            Assert.Equal("manager", product.ModifiedBy);

            _repositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _repositoryMock.Verify(
                x => x.UpdateAsync(product),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ProductDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var dto = new UpdateProductDto
            {
                ProductName = "New Name"
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _service.UpdateAsync(
                999,
                dto,
                "admin");

            // Assert
            Assert.False(result);

            _repositoryMock.Verify(
                x => x.UpdateAsync(It.IsAny<Product>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ProductExists_ReturnsTrue()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                ProductName = "Laptop",
                CreatedBy = "admin",
                CreatedOn = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(product);

            _repositoryMock
                .Setup(x => x.DeleteAsync(product))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);

            _repositoryMock.Verify(
                x => x.GetByIdAsync(1),
                Times.Once);

            _repositoryMock.Verify(
                x => x.DeleteAsync(product),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ProductDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);

            _repositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<Product>()),
                Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsProductsAndTotalCount()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    ProductName = "Laptop",
                    CreatedBy = "admin",
                    CreatedOn = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    ProductName = "Mouse",
                    CreatedBy = "admin",
                    CreatedOn = DateTime.UtcNow
                }
            };

            _repositoryMock
                .Setup(x => x.GetAllAsync(1, 10))
                .ReturnsAsync((products, 2));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Products.Count());

            Assert.Equal(
                "Laptop",
                result.Products.First().ProductName);

            _repositoryMock.Verify(
                x => x.GetAllAsync(1, 10),
                Times.Once);
        }
    }
}