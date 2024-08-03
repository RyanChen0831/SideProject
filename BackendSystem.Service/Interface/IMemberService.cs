
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Security;
using Microsoft.AspNetCore.Http;

namespace BackendSystem.Service.Interface
{
    public interface IMemberService
    {
        public Task<IEnumerable<MemberViewModel>> GetAllMember();
        public Task<MemberViewModel> GetMember(string account, string password);
        public Task<OperationResultDTO<string>> RegisterMember(MemberInfo member);
        public Task<bool> DeleteMember(MemberInfo member);
        public Task<bool> UpdateMember(MemberInfo member);
        public Task<bool> UpdateMemberVerificationStatus(int userId);
        public Task<OperationResultDTO<User>> VerifyEmail(string token);

        public int? GetUserId();

    }
}
