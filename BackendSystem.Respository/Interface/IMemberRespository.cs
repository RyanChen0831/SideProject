using BackendSystem.Respository.Dtos;

namespace BackendSystem.Respository.Interface
{
    public interface IMemberRespository
    {
        public Task<IEnumerable<MemberCondition>> GetAllMember();
        public Task<MemberCondition> GetMember(string account,string password);
        public Task<bool> Register(MemberCondition member);
        public Task<bool> DeleteMember(MemberCondition member);
        public Task<bool> UpdateMember(MemberCondition member);
        public Task<RegisterOperationResult> CheckRegistration(MemberCondition member);
        public Task<bool> UpdateMemberVerificationStatus(int userId);

    }
}
