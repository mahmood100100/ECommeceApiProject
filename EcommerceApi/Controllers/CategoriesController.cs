using AutoMapper;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Entities.DTO;
using Ecommerce.Core.IRepositories;
using Ecommerce.Core.IRepositories.IServices;
using Ecommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IFilesService filesService;

        public IUnitOfWork<Categories> UnitOfWork { get; }

        public CategoriesController(IUnitOfWork<Categories> unitOfWork, IMapper mapper , IFilesService filesService)
        {
            UnitOfWork = unitOfWork;
            this.mapper = mapper;
            this.filesService = filesService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await UnitOfWork.CategoriesRepository.GetAll(10, 1, "products");
                var categoriesDto = mapper.Map<IEnumerable<CategoriesResponseDTO>>(categories);
                return Ok(new ApiResponse(200, "Categories retrieved successfully", categoriesDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message,
            "An error occurred while processing the request"
        }, 500));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoriesRequestDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>
        {
            "Please provide valid category details."
        }, 400));
            }

            try
            {
                var category = mapper.Map<Categories>(categoryDto);

                await UnitOfWork.CategoriesRepository.Add(category);
                await UnitOfWork.Save();

                var createdCategoryDto = mapper.Map<CategoriesResponseDTO>(category);

                return CreatedAtAction(nameof(GetAllCategories), new { id = category.Id }, new ApiResponse(201, "Category created successfully", createdCategoryDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message,
            "An error occurred while processing the request"
        }, 500));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await UnitOfWork.CategoriesRepository.GetById(id);

                if (category == null)
                {
                    return NotFound(new ApiResponse(404, "Category not found"));
                }

                var categotyProducts = await UnitOfWork.ProductRepository.GetAllProductsByCategoryId(id);
                foreach (var product in categotyProducts)
                {
                    if(product.Image !=  null)
                    {
                        filesService.DeleteFile(product.Image , "UploadedFiles");
                    }
                }

                UnitOfWork.CategoriesRepository.Delete(id);
                await UnitOfWork.Save();

                return Ok(new ApiResponse(200, "Category and its products deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message,
            "An error occurred while processing the request"
        }, 500));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await UnitOfWork.CategoriesRepository.GetById(id);

                if (category == null)
                {
                    return NotFound(new ApiResponse(404, "Category not found"));
                }

                var categoryDto = mapper.Map<CategoriesResponseDTO>(category);
                return Ok(new ApiResponse(200, "Category retrieved successfully", categoryDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message,
            "An error occurred while processing the request"
        }, 500));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoriesRequestDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>
        {
            "Please provide valid category details."
        }, 400));
            }

            try
            {
                var existingCategory = await UnitOfWork.CategoriesRepository.GetById(id);

                if (existingCategory == null)
                {
                    return NotFound(new ApiResponse(404, "Category not found"));
                }

                mapper.Map(categoryDto, existingCategory);

                UnitOfWork.CategoriesRepository.Update(existingCategory);
                await UnitOfWork.Save();

                var updatedCategoryDto = mapper.Map<CategoriesResponseDTO>(existingCategory);

                return Ok(new ApiResponse(200, "Category updated successfully", updatedCategoryDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message,
            "An error occurred while processing the request"
        }, 500));
            }
        }



    }
}
