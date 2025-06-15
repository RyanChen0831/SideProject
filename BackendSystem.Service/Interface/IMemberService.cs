
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Security;

namespace BackendSystem.Service.Interface
{
    public interface IMemberService
    {
        public Task<IEnumerable<MemberResultModel>> GetAllMember();
        public Task<MemberResultModel> GetMember(string account, string password);
        public Task<MemberViewModel> GetMember(int memberId);
        public Task<bool> DeleteMember(MemberInfo member);
        public Task<bool> UpdateMember(MemberInfo member);
        public Task<bool> UpdateMemberVerificationStatus(int memberId);
        public Task<OperationResultDTO<string>> RegisterMember(MemberInfo member);
        public Task<OperationResultDTO<User>> VerifyEmail(string token);
        public int? GetMemberId();

    }
}
