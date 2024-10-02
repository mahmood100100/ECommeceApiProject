using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Image {  get; set; }
        public decimal Price { get; set; }

        [ForeignKey(nameof(categories))]
        public int CetegoryId { get; set; }
        public virtual Categories? categories { get; set; }
        public virtual ICollection<OrderDetails>? orderDetails { get; set; } = new HashSet<OrderDetails>();
    }
}
