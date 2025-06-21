
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;
using System.Data;

namespace BackendSystem.Respository.Interface
{
    public interface IMemberRespository
    {

        public Task<MemberCommandModel> GetMember(IDbConnection conn,string account, string password);
        public Task<MemberProfileResultModel> GetMember(IDbConnection conn, int memberId);
        public Task<bool> RegisterMember(IDbConnection conn, IDbTransaction tx, MemberCommandModel member);
        public Task<MemberDuplicationCheckResultModel> GetDuplicatedMemberInfo(IDbConnection conn, MemberDuplicationCheckResultModel member);


    }
}
