using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Dapper;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class MemberRespository : IMemberRespository
    {
        public MemberRespository()
        {
        }
        /// <summary>
        /// 驗證帳號或密碼是否重複註冊
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<bool> IsDuplicateAccountOrEmail(IDbConnection conn, string accountId, string mail)
        {
            string str = @"SELECT COUNT(*)
                           FROM Member
                           Where (Account=@Account OR Mail=@Mail) AND IsDeleted=0 ";
            var result = await conn.ExecuteScalarAsync<int>(str, new { Account = accountId, Mail = mail });
            return result > 0;
        }

        public async Task<MemberCommandModel?> GetMemberByAccount(IDbConnection conn, string account)
        {
            string sql = @" SELECT * FROM Member WHERE Account = @Account AND IsVerifyEmail= 'Y' AND IsDeleted=0 ";
            var parm = new DynamicParameters();
            parm.Add("Account", account, DbType.String);
            var members = await conn.QueryFirstOrDefaultAsync<MemberCommandModel>(sql, parm);
            return members;
        }

        public async Task<MemberProfileResultModel?> GetMember(IDbConnection conn, int memberId)
        {
            string sql = @" 
                        SELECT MB.Name,MB.Gender,CONVERT(VARCHAR, MB.Birthday, 23) AS Birthday,MB.Phone,MB.Address,MB.Mail,ML.LevelName AS Level, COALESCE(OD.TotalAmount, 0) AS TotalAmount
                        FROM Member MB
                        LEFT JOIN (
                            SELECT MemberId, SUM(TotalAmount) AS TotalAmount
                            FROM Orders
                            WHERE PaymentStatus = 'Completed'
                            GROUP BY MemberId
                        ) OD ON OD.MemberId = MB.MemberId
                        LEFT JOIN MemberLevel ML ON MB.LevelId = ML.LevelId 
                        WHERE MB.MemberId=@MemberId  ";
            var parm = new DynamicParameters();
            parm.Add("MemberId", memberId, DbType.Int32);
            var member = await conn.QueryFirstOrDefaultAsync<MemberProfileResultModel>(sql, parm);

            return member;
        }

        public async Task<MemberCommandModel> CreateMember(IDbConnection conn, IDbTransaction tx, MemberCommandModel member)
        {
            string sql = @"
                INSERT INTO Member (Name, Account, Password, Gender, Birthday, Phone, Address, Mail, Role, IsVerifyEmail) 
                VALUES (@Name, @Account, @Password, @Gender, @Birthday, @Phone, @Address, @Mail, 'User', 'N');
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            var newId = await conn.ExecuteScalarAsync<int>(sql, member, tx);
            member.MemberId = newId;
            return member;
        }
    }
}
