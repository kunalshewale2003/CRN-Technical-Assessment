using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize)
        {
            var result = await _productRepository.GetAllAsync(
                pageNumber,
                pageSize);

            var products = result.Products.Select(MapToDto);

            return (products, result.TotalCount);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            return product == null ? null : MapToDto(product);
        }

        public async Task<ProductDto> CreateAsync(
            CreateProductDto dto,
            string createdBy)
        {
            var product = new Product
            {
                ProductName = dto.ProductName,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow
            };

            var createdProduct =
                await _productRepository.AddAsync(product);

            return MapToDto(createdProduct);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateProductDto dto,
            string modifiedBy)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return false;
            }

            product.ProductName = dto.ProductName;
            product.ModifiedBy = modifiedBy;
            product.ModifiedOn = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return false;
            }

            await _productRepository.DeleteAsync(product);

            return true;
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CreatedBy = product.CreatedBy,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedOn = product.ModifiedOn
            };
        }
    }
}
