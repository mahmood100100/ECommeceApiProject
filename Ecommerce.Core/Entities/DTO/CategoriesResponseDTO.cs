using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities.DTO
{
    public class CategoriesResponseDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public IEnumerable<ProductResponseDTO> Products { get; set; }
    }
}
