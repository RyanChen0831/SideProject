using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Respository.Dtos
{
    public class OrderDataModel
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int SubTotal { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Payment { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
