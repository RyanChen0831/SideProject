
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModel;

namespace BackendSystem.Respository.Interface
{
    public interface IMemberRespository
    {

        public Task<MemberCommandModel> GetMember(string account, string password);
        public Task<MemberProfileResultModel> GetMember(int memberId);
        public Task<bool> Register(MemberCommandModel member);
        public Task<RegisterOperationResultModel> CheckRegistration(MemberCommandModel member);


    }
}
