using Application.DTOs;
using Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;
[Authorize]
[ApiController]
[Route("api/v1/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    /// <summary>
    /// Gets a paginated list of products.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Page number must be greater than 0."
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Page size must be between 1 and 100."
            });
        }

        var result = await _productService.GetAllAsync(
            pageNumber,
            pageSize);

        var response = new PagedResponse<ProductDto>
        {
            Items = result.Products,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = result.TotalCount
        };

        return Ok(new ApiResponse<PagedResponse<ProductDto>>
        {
            Success = true,
            Message = "Products retrieved successfully.",
            Data = response
        });
    }

    /// <summary>
    /// Gets a product by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Product not found."
            });
        }

        return Ok(new ApiResponse<ProductDto>
        {
            Success = true,
            Message = "Product retrieved successfully.",
            Data = product
        });
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => error.ErrorMessage)
                .ToList();

            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed.",
                Data = errors
            });
        }

        var createdBy = User.Identity?.Name ?? "Unknown";

        var product = await _productService.CreateAsync(
            dto,
            createdBy);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            new ApiResponse<ProductDto>
            {
                Success = true,
                Message = "Product created successfully.",
                Data = product
            });
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateProductDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => error.ErrorMessage)
                .ToList();

            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed.",
                Data = errors
            });
        }

        var modifiedBy = User.Identity?.Name ?? "Unknown";

        var updated = await _productService.UpdateAsync(
            id,
            dto,
            modifiedBy);

        if (!updated)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Product not found."
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Product updated successfully."
        });
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Product not found."
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Product deleted successfully."
        });
    }
}