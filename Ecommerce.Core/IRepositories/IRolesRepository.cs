using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.IRepositories
{
    public interface IRolesRepository
    {
        public Task AddRole(string roleName);
        public Task RemoveRole(string roleName);
        public Task UpdateRole(string id , string roleName);
        public Task<IEnumerable<IdentityRole>> GetAllRoles();
    }
}
