using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModel;

namespace BackendSystem.Respository.Interface
{
    public interface IMemberManagementRespository
    {
        public Task<IEnumerable<MemberManagementResultModel>> GetAllMember();
        public Task<int> DeleteMember(MemberManagementCommandModel member);
        public Task<int> UpdateMember(MemberManagementCommandModel member);
        public Task<int> UpdateMemberVerificationStatus(int memberId);
    }
}
