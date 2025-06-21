using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModels;
using Dapper;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class MemberRespository : IMemberRespository
    {
        public MemberRespository(IDbConnection dbConnection)
        {
        }

        public async Task<MemberDuplicationCheckResultModel> GetDuplicatedMemberInfo(IDbConnection conn, MemberDuplicationCheckResultModel member)
        {
            string str = @"SELECT Account,Phone,Mail
                           FROM Member
                           Where Account=@Account OR Phone=@Phone OR Mail=@Mail ";
            var parm = new DynamicParameters();
            parm.Add("Account", member.Account);
            parm.Add("Phone", member.Phone);
            parm.Add("Mail", member.Mail);

            var result = await conn.QueryFirstOrDefaultAsync<MemberDuplicationCheckResultModel>(str, parm);
            return result;
        }

        public async Task<MemberCommandModel> GetMember(IDbConnection conn, string account, string password)
        {
            string sql = @" SELECT * FROM Member WHERE Account = @Account AND Password = @Password ";
            var parm = new DynamicParameters();
            parm.Add("Account", account, DbType.String);
            parm.Add("Password", password, DbType.String);
            var members = await conn.QueryFirstOrDefaultAsync<MemberCommandModel>(sql, parm);
            return members;
        }

        public async Task<MemberProfileResultModel> GetMember(IDbConnection conn, int memberId)
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

        public async Task<bool> RegisterMember(IDbConnection conn, IDbTransaction tx, MemberCommandModel member)
        {
            string sql = @"INSERT INTO Member (Name, Account, Password, Gender, Birthday, Phone, Address, Mail, Role,IsVerifyEmail) 
                   VALUES (@Name, @Account, @Password, @Gender, @Birthday, @Phone, @Address, @Mail, @Role,'N')";
            var parm = new DynamicParameters();
            parm.AddDynamicParams(member);
            try
            {
                await conn.ExecuteAsync(sql, parm, tx);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
