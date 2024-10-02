using Ecommerce.Core.IRepositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesRepository(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public async Task AddRole(string roleName)
        {
            var identityRole = new IdentityRole(roleName);
            var result = await roleManager.CreateAsync(identityRole);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to create role");
            }
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRoles()
        {
            return await Task.FromResult(roleManager.Roles.ToList());
        }

        public async Task RemoveRole(string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            var result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new Exception("Failed to delete role");
        }

        public async Task UpdateRole(string id, string newRoleName)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            role.Name = newRoleName;

            var result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                throw new Exception("Failed to update role");
        }
    }
}
