using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities.DTO
{
    public class OrderDetailsRequestDTO
    {
        public int OrderId {  get; set; } 
        public int ProductId { get; set; }
        public int quantity {  get; set; }
    }
}
