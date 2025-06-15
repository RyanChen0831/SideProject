namespace BackendSystem.Respository.CommandModels
{
    public class MemberManagementCommandModel
    {
        public int MemberId { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Mail { get; set; }
        public string UpdateBy { get; set; }

    }
}
