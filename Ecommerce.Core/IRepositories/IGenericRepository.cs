using Ecommerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        public Task<IEnumerable<T>> GetAll(int PageSize = 2 , int PageNumber = 1 , string? IncludeProperty = null , Expression<Func<T, bool>> filter = null);
        public Task<T> GetById(int id);
        public Task Add(T model);
        public void Update(T model);
        public void Delete(int id);
    }
}
