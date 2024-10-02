using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Core.Entities.DTO
{
    public class ProductRequestDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
        public int CetegoryId { get; set; }
    }
}
