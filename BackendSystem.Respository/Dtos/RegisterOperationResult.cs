using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Respository.Dtos
{
    public class RegisterOperationResult
    {
        public RegisterOperationResult()
        {
        }
        public RegisterOperationResult(bool isSucceed,string message) 
        {
            IsSucceed = isSucceed;
            Message= message;
        }
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
    }
}
