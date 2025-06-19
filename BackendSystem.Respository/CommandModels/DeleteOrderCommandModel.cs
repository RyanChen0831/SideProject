namespace BackendSystem.Respository.CommandModels
{
    public class DeleteOrderCommandModel
    {
        public string OrderId { get; set; }
        public string DeletedBy { get; set; }
    }
}
