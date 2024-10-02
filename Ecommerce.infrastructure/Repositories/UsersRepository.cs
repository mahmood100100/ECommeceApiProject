using AutoMapper;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Entities.DTO;
using Ecommerce.Core.IRepositories;
using Ecommerce.Core.IRepositories.IServices;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext dbContext;
        private readonly UserManager<LocalUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<LocalUser> signInManager;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public UsersRepository(AppDbContext dbContext,
            UserManager<LocalUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<LocalUser> signInManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        public IEnumerable<LocalUserDTO> GetAllUsers()
        {
            var users = userManager.Users.ToList();

            var userDtos = mapper.Map<IEnumerable<LocalUserDTO>>(users);

            return userDtos;
        }

        public async Task<LocalUserDTO> GetUserById(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            var userDto = mapper.Map<LocalUserDTO>(user);

            return userDto;
        }

        public bool IsUniqueUser(string username, string email)
        {
            var result = dbContext.LocalUser.FirstOrDefault(x => x.UserName == username || x.Email == email);
            return result == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDTO.Email);
            if (user == null)
            {
                throw new Exception("Invalid user email , this email does not register");
            }

            var CheckingPassword = await signInManager.CheckPasswordSignInAsync(user, loginRequestDTO.Password , false);
            if (!CheckingPassword.Succeeded)
            {
                throw new Exception("Invalid user password , remember it and try again.");
            }

            var roles = await userManager.GetRolesAsync(user);
            var RolesString = String.Join('_', roles);

            return new LoginResponseDTO
            {
                User = mapper.Map<LocalUserDTO>(user), 
                Token = await tokenService.CreateTokenAsync(user),
                Role = RolesString
            };
        }

        public async Task<LocalUserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            if(registrationRequestDTO.Role.Equals("Admin" , StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("cannot assign the admin role to a user.");
            }

            var user = new LocalUser
            {
                UserName = registrationRequestDTO.UserName,
                Email = registrationRequestDTO.Email,
                FirstName = registrationRequestDTO.FirstName,
                LastName = registrationRequestDTO.LastName,
                Address = registrationRequestDTO.Address,
            };

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await userManager.CreateAsync(user, registrationRequestDTO.Password);
                    if (result.Succeeded)
                    {
                        var isRoleExist = await roleManager.RoleExistsAsync(registrationRequestDTO.Role);
                        if (!isRoleExist)
                        {
                            throw new Exception($"The role {registrationRequestDTO.Role} does not exist!");
                        }

                        var userRoleResult = await userManager.AddToRoleAsync(user, registrationRequestDTO.Role);
                        if (userRoleResult.Succeeded)
                        {
                            await transaction.CommitAsync();
                            var returnedUser = dbContext.LocalUser.FirstOrDefault(x => x.UserName == registrationRequestDTO.UserName);
                            return mapper.Map<LocalUserDTO>(returnedUser);
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            throw new Exception("Failed to add user to usersRoles");
                        }
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        throw new Exception("User registration failed");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    throw;
                }
            }
        }
    }
}
