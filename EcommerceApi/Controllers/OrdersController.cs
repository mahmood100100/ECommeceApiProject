using AutoMapper;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Entities.DTO;
using Ecommerce.Core.IRepositories;
using Ecommerce.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork<Orders> unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<LocalUser> userManager;
        private ApiResponse response;

        public OrdersController(IUnitOfWork<Orders> unitOfWork, IMapper mapper , UserManager<LocalUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
            this.response = new ApiResponse();
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllOrders()
        {
            try
            {
                var orders = await unitOfWork.OrderRepository.GetAll(10, 1, "orderDetails");
                if (orders.Any())
                {
                    var orderDtos = mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
                    response.StatusCode = 200;
                    response.IsSuccess = true;
                    response.Message = "Orders retrived successfully";
                    response.Result = orderDtos;
                }
                else
                {
                    response.StatusCode = 204;
                    response.IsSuccess = false;
                    response.Message = "No orders found.";
                }
                return Ok(response);
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
        public async Task<ActionResult<ApiResponse>> GetOrderById(int id)
        {
            try
            {
                var order = await unitOfWork.OrderRepository.GetById(id);
                if (order != null)
                {
                    var orderDto = mapper.Map<OrderResponseDTO>(order);
                    response.StatusCode = 200;
                    response.IsSuccess = true;
                    response.Message = "Order retrived successfully";
                    response.Result = orderDto;
                }
                else
                {
                    response.StatusCode = 404;
                    response.IsSuccess = false;
                    response.Message = "Order not found.";
                }
                return Ok(response);
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

        [HttpPost("{userId}")]
        public async Task<ActionResult<ApiResponse>> CreateOrder(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>
        {
            "Please provide valid order details."
        }, 400));
            }

            try
            {
                var userExists = await userManager.FindByIdAsync(userId);
                if (userExists == null)
                {
                    return BadRequest(new ApiValidationResponse(new List<string>
            {
                "User does not exist."
            }, 400));
                }

                var order = new Orders
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending"
                };

                await unitOfWork.OrderRepository.Add(order);
                await unitOfWork.Save();

                var createdOrderDto = mapper.Map<OrderResponseDTO>(order);

                var response = new ApiResponse
                {
                    StatusCode = 201,
                    IsSuccess = true,
                    Result = createdOrderDto,
                    Message = "Order created successfully."
                };

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, response);
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
        public async Task<ActionResult<ApiResponse>> UpdateOrder(int id, [FromQuery] string status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>
                {
                    "Please provide valid order details."
                }, 400));
            }

            try
            {
                var existingOrder = await unitOfWork.OrderRepository.GetById(id);
                if (existingOrder == null)
                {
                    return NotFound(new ApiResponse(404, "Order not found."));
                }

                existingOrder.Status = status;
                unitOfWork.OrderRepository.Update(existingOrder);
                await unitOfWork.Save();

                var updatedOrderDto = mapper.Map<OrderResponseDTO>(existingOrder);
                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Result = updatedOrderDto;
                response.Message = "Order updated successfully.";

                return Ok(response);
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
        public async Task<ActionResult<ApiResponse>> DeleteOrder(int id)
        {
            try
            {
                var order = await unitOfWork.OrderRepository.GetById(id);
                if (order == null)
                {
                    return NotFound(new ApiResponse(404, "Order not found."));
                }

                unitOfWork.OrderRepository.Delete(id);
                await unitOfWork.Save();

                response.StatusCode = 200;
                response.IsSuccess = true;
                response.Message = "Order and its details deleted successfully.";

                return Ok(response);
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

        [HttpGet("/User/{User_Id}")]
        public async Task<ActionResult<ApiResponse>> GetOrdersForUser(string User_Id)
        {
            try
            {
                var orders = await unitOfWork.OrderRepository.GetAllOrdersByUserId(User_Id);
                if (orders.Any())
                {
                    var orderDtos = mapper.Map<IEnumerable<OrderResponseDTO>>(orders);
                    response.StatusCode = 200;
                    response.IsSuccess = true;
                    response.Message = "Orders retrived successfully";
                    response.Result = orderDtos;
                }
                else
                {
                    response.StatusCode = 204;
                    response.IsSuccess = false;
                    response.Message = "No orders found for this user.";
                }
                return Ok(response);
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
