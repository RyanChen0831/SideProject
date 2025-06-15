using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Service.Dtos
{
    public class OrderDetailInfo
    {
        public string? OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int SubTotal { get; set; }

    }
}
