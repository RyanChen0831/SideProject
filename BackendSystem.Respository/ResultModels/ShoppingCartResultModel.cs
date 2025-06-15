namespace BackendSystem.Respository.ResultModels
{
    public class ShoppingCartResultModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int SubTotal { get; set; }
    }
}
