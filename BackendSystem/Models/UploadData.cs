namespace BackendSystem.Models
{
    public class UploadData
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public List<IFormFile> Path { get; set; }
        public string ImgDescription { get; set; }

    }
}
