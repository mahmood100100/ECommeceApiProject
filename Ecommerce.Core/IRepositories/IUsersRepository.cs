using Ecommerce.Core.Entities;
using Ecommerce.Core.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.IRepositories
{
    public interface IUsersRepository
    {
        Task<LoginResponseDTO>Login(LoginRequestDTO loginRequestDTO);
        Task<LocalUserDTO>Register(RegistrationRequestDTO registrationRequestDTO);
        bool IsUniqueUser(string username , string email);
        IEnumerable<LocalUserDTO> GetAllUsers();
        Task<LocalUserDTO> GetUserById(string id);
    }
}
