using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ecommerce.Core.IRepositories
{
    public interface IUnitOfWork<T> where T : class
    {
        public IProductRepository ProductRepository { get; set; }
        public ICategoriesRepository CategoriesRepository { get; set; }
        public IOrderRepository OrderRepository { get; set; }
        public IOrderDetailsRepository OrderDetailsRepository { get; set; }
        public Task<int> Save();
    }
}
