using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetAllAsync(
            int pageNumber,
            int pageSize);

        Task<ProductDto?> GetByIdAsync(int id);

        Task<ProductDto> CreateAsync(
            CreateProductDto dto,
            string createdBy);

        Task<bool> UpdateAsync(
            int id,
            UpdateProductDto dto,
            string modifiedBy);

        Task<bool> DeleteAsync(int id);
    }
}
