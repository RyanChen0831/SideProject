using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Service.Dtos
{
    public class ProductInfo
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public int Price { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
    }
}
