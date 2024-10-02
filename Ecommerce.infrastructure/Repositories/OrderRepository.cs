using Ecommerce.Core.Entities;
using Ecommerce.Core.IRepositories;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Orders>, IOrderRepository
    {
        private readonly AppDbContext dbContext;
        public OrderRepository(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Orders>> GetAllOrdersByUserId(string User_Id)
        {
            var orders = await dbContext.Orders
                .Where(o => o.UserId == User_Id).ToListAsync();
            return orders;
        }
    }
}
