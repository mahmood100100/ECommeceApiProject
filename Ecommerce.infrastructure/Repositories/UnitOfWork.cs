using Ecommerce.Core.IRepositories;
using Ecommerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly AppDbContext dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            ProductRepository = new ProductRepository(dbContext);
            CategoriesRepository = new CategoriesRepository(dbContext);
            OrderRepository = new OrderRepository(dbContext);
            OrderDetailsRepository = new OrderDetailsRepository(dbContext);
        }
        public IProductRepository ProductRepository { get; set; }
        public ICategoriesRepository CategoriesRepository { get; set; }
        public IOrderRepository OrderRepository { get ; set; }
        public IOrderDetailsRepository OrderDetailsRepository { get; set; }

        public async Task<int> Save() => await dbContext.SaveChangesAsync();
    }
}
