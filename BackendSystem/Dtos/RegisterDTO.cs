using System.ComponentModel.DataAnnotations;

namespace BackendSystem.Dtos
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(30, ErrorMessage = "輸入字元超出限制")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "輸入字元超出限制")]
        public string Account { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "輸入字元超出限制")]
        public string Password { get; set; }
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }
        public string Address { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "格式錯誤")]
        public string Mail { get; set; }

    }
}
