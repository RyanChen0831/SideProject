using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Respository.Dtos
{
    public class ShoppingCartCondition
    {
        public int? UserID { get; set; }
        public string? NonMemberToken { get; set; }

    }
}
