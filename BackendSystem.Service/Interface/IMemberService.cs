
using BackendSystem.Service.QueryModels;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.ResultModels;
using BackendSystem.Common.Dtos;

namespace BackendSystem.Service.Interface
{
    public interface IMemberService
    {
        public Task<MemberResultModel> ValidateLoginAsync(string account, string password);
        public Task<MemberViewModel?> GetMember(int memberId);
        public Task<OperationResultDTO<string>> RegisterMember(MemberRegisterModel member);
        public Task<OperationResultDTO<User>> VerifyEmail(string token);

    }
}
