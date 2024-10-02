using Ecommerce.Core.Entities;
using Ecommerce.Core.IRepositories;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext dbContext;
        public GenericRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task Add(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            await dbContext.Set<T>().AddAsync(model);
        }

        public void Delete(int id)
        {
            var model = dbContext.Set<T>().Find(id);
            if (model == null)
                throw new KeyNotFoundException($"Entity with ID {id} not found");

            dbContext.Set<T>().Remove(model);
        }

        public async Task<IEnumerable<T>> GetAll(int pageSize, int pageNumber, string? includeProperties = null, Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = dbContext.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(item);
                }
            }

            pageSize = pageSize > 0 ? Math.Min(pageSize, 100) : 10;
            pageNumber = pageNumber > 0 ? pageNumber : 1;

            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            var model = await dbContext.Set<T>().FindAsync(id);
            return model;
        }

        public void Update(T model)
        {
            dbContext.Set<T>().Update(model);
        }
    }

}
