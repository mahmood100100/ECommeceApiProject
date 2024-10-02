using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities.DTO
{
    public class OrderDetailsResponseDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int quantity { get; set; }
        public decimal Price { get; set; }
        public ProductResponseDTO product { get; set; }
    } 
}
