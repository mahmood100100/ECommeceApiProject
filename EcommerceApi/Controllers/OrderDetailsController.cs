using AutoMapper;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Entities.DTO;
using Ecommerce.Core.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IUnitOfWork<OrderDetails> unitOfWork;
        private readonly IMapper mapper;

        public OrderDetailsController(IUnitOfWork<OrderDetails> unitOfWork , IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllOrderDetails()
        {
            try
            {
                var orderDetails = await unitOfWork.OrderDetailsRepository.GetAll(10, 1, "products");

                var orderDetailsDto = mapper.Map<IEnumerable<OrderDetailsResponseDTO>>(orderDetails);

                return Ok(new ApiResponse(200, "Order details retrieved successfully", orderDetailsDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message,
            "An error occurred while processing the request."
        }, 500));
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetOrderDetailsById(int id)
        {
            try
            {
                var orderDetail = await unitOfWork.OrderDetailsRepository.GetById(id);

                if (orderDetail == null)
                {
                    return NotFound(new ApiResponse(404, "Order detail not found"));
                }

                var orderDetailDto = mapper.Map<OrderDetailsResponseDTO>(orderDetail);

                return Ok(new ApiResponse(200, "Order detail retrieved successfully", orderDetailDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
                {
                    ex.Message,
                    "An error occurred while processing the request."
                }, 500));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateOrderDetails([FromBody] OrderDetailsRequestDTO orderDetailsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>
        {
            "Please provide valid order detail information."
        }, 400));
            }

            try
            {
                var product = await unitOfWork.ProductRepository.GetById(orderDetailsDto.ProductId);
                if (product == null)
                {
                    return NotFound(new ApiResponse(404, "Product not found"));
                }

                var orderDetail = mapper.Map<OrderDetails>(orderDetailsDto);

                orderDetail.Price = orderDetailsDto.quantity * product.Price;

                await unitOfWork.OrderDetailsRepository.Add(orderDetail);
                await unitOfWork.Save();

                var createdOrderDetailDto = mapper.Map<OrderDetailsResponseDTO>(orderDetail);

                return CreatedAtAction(nameof(GetOrderDetailsById), new { id = orderDetail.Id }, new ApiResponse(201, "Order detail created successfully", createdOrderDetailDto));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message,
            "An error occurred while processing the request."
        }, 500));
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> UpdateOrderDetails(int id, [FromBody] OrderDetailsRequestDTO orderDetailsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>
        {
            "Please provide valid order detail information."
        }, 400));
            }

            try
            {
                var orderDetail = await unitOfWork.OrderDetailsRepository.GetById(id);

                if (orderDetail == null)
                {
                    return NotFound(new ApiResponse(404, "Order detail not found"));
                }

                mapper.Map(orderDetailsDto, orderDetail);

                var product = await unitOfWork.ProductRepository.GetById(orderDetail.ProductId);
                if (product != null)
                {
                    orderDetail.Price = orderDetailsDto.quantity * product.Price;
                }

                unitOfWork.OrderDetailsRepository.Update(orderDetail);
                await unitOfWork.Save();

                return Ok(new ApiResponse(200, "Order detail updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
        {
            ex.Message,
            "An error occurred while processing the request."
        }, 500));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteOrderDetails(int id)
        {
            try
            {
                var orderDetail = await unitOfWork.OrderDetailsRepository.GetById(id);

                if (orderDetail == null)
                {
                    return NotFound(new ApiResponse(404, "Order detail not found"));
                }

                unitOfWork.OrderDetailsRepository.Delete(id);
                await unitOfWork.Save();

                return Ok(new ApiResponse(200, "Order detail deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>
                {
                    ex.Message,
                    "An error occurred while processing the request."
                }, 500));
            }
        }


    }
}
