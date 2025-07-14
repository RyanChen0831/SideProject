
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.ResultModels;
using System.Data;

namespace BackendSystem.Respository.Interface
{
    public interface IMemberRespository
    {

        public Task<MemberCommandModel?> GetMemberByAccount(IDbConnection conn,string account);
        public Task<MemberProfileResultModel?> GetMember(IDbConnection conn, int memberId);
        public Task<MemberCommandModel> CreateMember(IDbConnection conn, IDbTransaction tx, MemberCommandModel member);
        public Task<bool> IsDuplicateAccountOrEmail(IDbConnection conn, string accountId, string mail);


    }
}
