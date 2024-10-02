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
    public class ProductRepository : GenericRepository<Products> , IProductRepository
    {
        private readonly AppDbContext dbContext;

        public ProductRepository(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Products>> GetAllProductsByCategoryId(int Cat_id)
        {
            //Eager loading
            // var products = (IEnumerable<Products>) await dbContext.Products.Include(x => x.categories)
            //    .Where(c => c.CetegoryId == Cat_id).ToListAsync();
            // return products;

            // Explicit loading
            //var products = await dbContext.Products
            //    .Where(c => c.CetegoryId == Cat_id).ToListAsync();
            //foreach (var product in products)
            //{
            //    await dbContext.Entry(product).Reference(r => r.categories).LoadAsync();
            //}
            //return products;

            // lazy loading
            var products =  await dbContext.Products
                .Where(p => p.CetegoryId == Cat_id).ToListAsync();
            return products;

        }
    }
}
