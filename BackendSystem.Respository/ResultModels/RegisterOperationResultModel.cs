namespace BackendSystem.Respository.ResultModels
{
    public class RegisterOperationResultModel
    {
        public RegisterOperationResultModel(bool isSucceed, string message)
        {
            IsSucceed = isSucceed;
            Message = message;
        }
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
    }
}
