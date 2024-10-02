using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities.DTO
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
