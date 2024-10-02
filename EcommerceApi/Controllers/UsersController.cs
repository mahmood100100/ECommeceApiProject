using Ecommerce.Core.Entities;
using Ecommerce.Core.Entities.DTO;
using Ecommerce.Core.IRepositories;
using Ecommerce.Core.IRepositories.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository usersRepository;
        private readonly UserManager<LocalUser> userManager;
        private readonly IEmailService emailService;

        public UsersController(IUsersRepository usersRepository , UserManager<LocalUser> userManager , IEmailService emailService)
        {
            this.usersRepository = usersRepository;
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegistrationRequestDTO user)
        {
            try
            {
                var IsUserExist = !usersRepository.IsUniqueUser(user.UserName, user.Email);
                if(IsUserExist)
                {
                    return BadRequest(new ApiResponse(400 , "Email or UserName already exist"));
                }

                var addedUser = await usersRepository.Register(user);
                if(addedUser == null)
                {
                    return BadRequest(new ApiResponse(400, "failed response while adding the user , try again"));
                }

                return Ok(new ApiResponse(201 , Result : addedUser));
            }catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiValidationResponse(new List<string>()
                {
                    ex.Message,
                    "error occured while proccesing the request"
                }, 500));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>()
                {
                    "please use the correct format"
                }, 400));
            }
            try
            {
                var loginResponse = await usersRepository.Login(loginRequest);
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return Unauthorized(new ApiValidationResponse(new List<string>()
                {
                    ex.Message
                } , 401));
            }
        }

        [HttpPost("sendMail")]
        public async Task<IActionResult> SendMail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new ApiValidationResponse(new List<string>()
        {
            $"this email {email} not found in our app"
        }, 400));
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var forgetPasswordLink = Url.Action("ResetPassword", "Users", new { Token = token, Email = email }, Request.Scheme);
            var subject = "Reset Password Request";
            var message = $"please click on the link to reset your password {forgetPasswordLink}";
            await emailService.SendMailAsync(email, subject, message);

            return Ok(new ApiResponse(200, "password reset link has been sent to your email. Check your email", token));
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDTO passwordDTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiValidationResponse(new List<string>()
                {
                    "write in correct format"
                } , 400));
            }

            if (string.Compare(passwordDTO.NewPassword , passwordDTO.ConfirmedNewPassword) != 0)
            {
                return BadRequest(new ApiResponse(400 , "passwords not matched"));
            }

            if(string.IsNullOrEmpty(passwordDTO.Token))
            {
                return BadRequest(new ApiResponse(400 , "token missed or invalid"));
            }

            var user = await userManager.FindByEmailAsync(passwordDTO.Email);
            if (user == null)
            {
                return NotFound(new ApiResponse(404 , "email incorrect"));
            }

            var result = await userManager.ResetPasswordAsync(user , passwordDTO.Token , passwordDTO.NewPassword);
            if(result.Succeeded)
            {
                return Ok(new ApiResponse(200 , "password reset successfully"));
            }
            else
            {
                return BadRequest(new ApiResponse(400 , "invalid token , try again"));
            }
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = usersRepository.GetAllUsers();
                return Ok(new ApiResponse(200, "Users retrieved successfully", users));
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

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await usersRepository.GetUserById(id);
                return Ok(new ApiResponse(200, "User retrieved successfully", user));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse(404, ex.Message));
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
