using AutoMapper;
using Ecommerce.Api.Mapping_Profiles;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Entities.DTO;
using Ecommerce.Core.IRepositories;
using Ecommerce.Core.IRepositories.IServices;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Infrastructure.Repositories;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public IUnitOfWork<Products> UnitOfWork { get; }
        public ApiResponse response;
        private readonly IMapper mapper;
        private readonly IFilesService filesService;

        public ProductController(IUnitOfWork<Products> UnitOfWork , IMapper mapper , IFilesService filesService)
        {
            this.UnitOfWork = UnitOfWork;
            this.mapper = mapper;
            this.filesService = filesService;
            this.response = new ApiResponse();
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "DefaultCache")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> GetAllProducts([FromQuery] string? CategoryName = null, int PageSize = 2, int PageNumber = 1)
        {
            Expression<Func<Products, bool>> filter = null;
            if (!string.IsNullOrEmpty(CategoryName))
            {
                filter = x => x.categories.Name.Contains(CategoryName);
            }

            var model = await UnitOfWork.ProductRepository.GetAll(PageSize: PageSize, PageNumber: PageNumber, IncludeProperty: "categories", filter);
            var check = model.Any();
            if (check)
            {
                var mappedProducts = mapper.Map<IEnumerable<Products>, IEnumerable<ProductResponseDTO>>(model);

                return Ok(new ApiResponse(200, "Products retrieved successfully", mappedProducts));
            }
            else
            {
                return Ok(new ApiResponse(200, "No products found", new List<ProductResponseDTO>()));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiValidationResponse(new List<string>
            {
                "Invalid ID",
            }, 400));
                }

                var model = await UnitOfWork.ProductRepository.GetById(id);
                if (model == null)
                {
                    return NotFound(new ApiResponse(404, "No product found with this ID"));
                }

                var productDto = mapper.Map<ProductResponseDTO>(model);

                return Ok(new ApiResponse(200, "Product retrieved successfully", productDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message
        }, 500));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddProduct([FromForm] ProductRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>
        {
            "Invalid data"
        }, 400));
            }

            var mappedProduct = mapper.Map<Products>(model);
            mappedProduct.CetegoryId = model.CetegoryId;

            try
            {
                string imageUrl = null;
                if (model.Image != null && model.Image.Length > 0)
                {
                    imageUrl = await filesService.UploadFileAsync(model.Image , "UploadedFiles");
                }

                mappedProduct.Image = imageUrl;
                var productResDto = mapper.Map<ProductResponseDTO>(mappedProduct);

                await UnitOfWork.ProductRepository.Add(mappedProduct);
                await UnitOfWork.Save();

                return Ok(new ApiResponse(200, "Product added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiValidationResponse(new List<string>
        {
            $"Failed to add product: {ex.Message}"
        }, 500));
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id ,[FromBody] ProductRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse
                {
                    StatusCode = 400,
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            var existingProduct = await UnitOfWork.ProductRepository.GetById(id);
            if (existingProduct == null)
            {
                return NotFound(new ApiResponse(404, "Product not found"));
            }

            var updatedProduct = mapper.Map(model, existingProduct);

            if (model.Image != null && model.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    filesService.DeleteFile(existingProduct.Image , "UploadedFiles");
                }

                updatedProduct.Image = await filesService.UploadFileAsync(model.Image, "UploadedFiles");
            }
            else
            {
                updatedProduct.Image = existingProduct.Image;
            }

            UnitOfWork.ProductRepository.Update(updatedProduct);
            await UnitOfWork.Save();

            return Ok(new ApiResponse(200, "Product updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await UnitOfWork.ProductRepository.GetById(id);
            if (product == null)
            {
                return NotFound(new ApiResponse(404, "Product not found"));
            }

            if (!string.IsNullOrEmpty(product.Image))
            {
                filesService.DeleteFile(product.Image, "UploadedFiles");
            }

            UnitOfWork.ProductRepository.Delete(id);
            await UnitOfWork.Save();

            return Ok(new ApiResponse(200, "Product deleted successfully"));
        }

        [HttpGet("category/{Category_Id}")]
        public async Task<ActionResult<ApiResponse>> GetAllByCategoryId(int Category_Id)
        {
            var model = await UnitOfWork.ProductRepository.GetAllProductsByCategoryId(Category_Id);
            var check = model.Any();
            if (check)
            {
                response.StatusCode = 200;
                response.IsSuccess = true;
                var mappedProducts = mapper.Map<IEnumerable<Products>, IEnumerable<ProductResponseDTO>>(model);
                response.Result = mappedProducts;
            }
            else
            {
                response.Message = "no products found in this category";
                response.StatusCode = 200;
                response.IsSuccess = false;
            }
            return Ok(response);
        }


    }
}
