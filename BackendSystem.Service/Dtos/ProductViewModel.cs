namespace BackendSystem.Service.Dtos
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Price { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
        public double? Rate { get; set; }
        public string? EnableTag { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? Category { get; set; }
        public string? ProductImage { get; set; }
        public List<string>? CategoryList { get; set; }
        public List<string>? ProductImageList { get; set; }
    }
}
