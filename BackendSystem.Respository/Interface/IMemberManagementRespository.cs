using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;
using System.Data;

namespace BackendSystem.Respository.Interface
{
    public interface IMemberManagementRespository
    {
        public Task<IEnumerable<MemberManagementResultModel>> GetAllMember(IDbConnection conn);
        public Task<int> DeleteMember(IDbConnection conn,IDbTransaction tx,int memberId);
        public Task<int> UpdateMember(IDbConnection conn, IDbTransaction tx, MemberManagementCommandModel member);
        public Task<int> UpdateMemberVerificationStatus(IDbConnection conn, IDbTransaction tx, int memberId);
    }
}
