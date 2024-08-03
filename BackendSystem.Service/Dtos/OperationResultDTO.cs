using System;

namespace BackendSystem.Service.Dtos
{
    public class OperationResultDTO<T>
    {
        public bool IsSucceed { get; set; }
        public string? Message { get; set; }
        public T? Object { get; set; } // 使用泛型類型 T

        public OperationResultDTO()
        {
        }

        public OperationResultDTO(bool isSucceed, T obj, string? message = null)
        {
            IsSucceed = isSucceed;
            Object = obj;
            Message = message;
        }

        public OperationResultDTO(bool isSucceed, string? message)
        {
            IsSucceed = isSucceed;
            Message = message;
        }
    }
}
