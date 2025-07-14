
using BackendSystem.Service.QueryModels;
using BackendSystem.Service.ResultModels;

namespace BackendSystem.Service.Interface
{
    interface IMemberManagementService
    {
        public Task<IEnumerable<MemberResultModel>> GetAllMembersAsync();
        public Task<bool> DeleteMemberAsync(int memberId);
        public Task<bool> UpdateMemberAsync(MemberQueryModel member);
    }
}
