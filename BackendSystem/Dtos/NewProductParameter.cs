using System.ComponentModel.DataAnnotations;

namespace BackendSystem.Dtos
{
    public class NewProductParameter
    {
        [Required]
        [StringLength(maximumLength:100,ErrorMessage ="輸入超出限制")]
        public string ProductName { get; set;}
        public string? Description { get; set;}
        public List<int> Category { get; set;}

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "不可小於1")]
        public int Price { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "不可小於1")]
        public int Stock { get; set; }


    }
}
