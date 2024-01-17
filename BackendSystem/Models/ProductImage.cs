namespace BackendSystem.Models
{
    public class ProductImage
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public List<IFormFile> Path { get; set; }
        public string ImgDescription { get; set; }

    }
}
