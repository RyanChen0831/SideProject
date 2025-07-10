namespace BackendSystem.Service.QueryModels
{
    public class MemberRegisterModel
    {
        public string Name { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Mail { get; set; }
    }

}
