namespace BackendSystem.Service.Dtos
{
    public class ProductInfo
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Price { get; set; }
        public List<int>? Category { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }

    }
}
