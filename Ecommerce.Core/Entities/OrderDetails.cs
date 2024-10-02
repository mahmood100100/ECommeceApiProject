using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class OrderDetails
    {
        public int Id { get; set; }
        [ForeignKey(nameof(orders))]
        public int OrderId { get; set; }
        [ForeignKey(nameof(products))]
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal quantity { get; set; }
        public virtual Products? products { get; set; }
        public virtual Orders? orders { get; set; }

    }
}
