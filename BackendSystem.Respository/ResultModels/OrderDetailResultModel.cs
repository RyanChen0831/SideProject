namespace BackendSystem.Respository.ResultModels
{
    public class OrderDetailResultModel
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public int SubTotal { get; set; }
    }
}
