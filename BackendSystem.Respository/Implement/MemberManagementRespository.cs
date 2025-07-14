
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using Dapper;
using BackendSystem.Respository.ResultModels;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class MemberManagementRespository : IMemberManagementRespository
    {
        public MemberManagementRespository()
        {
        }
        public async Task<IEnumerable<MemberManagementResultModel>> GetAllMember(IDbConnection conn)
        {
            string sql = @"
                        SELECT 
	                        Name,
	                        Account,
	                        Gender,
	                        Convert(varchar,Birthday,111) AS Birthday,
	                        [Phone], 
	                        [Address], 
	                        [Mail], 
	                        [Role], 
	                        [IsVerifyEmail], 
	                        Lev.LevelName, 
	                        [LevelExpireDate],
	                        IsDeleted
                        FROM Member Main
                        LEFT JOIN MemberLevel Lev ON Lev.LevelId = Main.LevelId
                        WHERE IsDeleted = 0";

            var members = await conn.QueryAsync<MemberManagementResultModel>(sql);
            return members;
        }
        public async Task<int> DeleteMember(IDbConnection conn,IDbTransaction tx,int memberId)
        {
            string sql = @"UPDATE Member SET IsDeleted = 1 WHERE MemberId = @MemberId AND IsDeleted = 0";
            return await conn.ExecuteAsync(sql, memberId, tx);
        }
        public async Task<int> UpdateMember(IDbConnection conn, IDbTransaction tx, MemberManagementCommandModel member)
        {
            string sql = @"
            UPDATE Member
            SET 
                Name = @Name,
                Account = @Account,
                Gender = @Gender,
                Birthday = @Birthday,
                Phone = @Phone,
                Address = @Address,
                Mail = @Mail,
                UpdateDate = GETDATE(),
                UpdateBy = @UpdateBy
            WHERE MemberId = @MemberId
              AND (
                ISNULL(Name, '')        != ISNULL(@Name, '')
                OR ISNULL(Account, '')  != ISNULL(@Account, '')
                OR ISNULL(Gender, '')   != ISNULL(@Gender, '')
                OR COALESCE(CONVERT(varchar, Birthday, 112), '') != COALESCE(CONVERT(varchar, @Birthday, 112), '')
                OR ISNULL(Phone, '')    != ISNULL(@Phone, '')
                OR ISNULL(Address, '')  != ISNULL(@Address, '')
                OR ISNULL(Mail, '')     != ISNULL(@Mail, '')
              )";
             return await conn.ExecuteAsync(sql, member, tx);
        }
        public async Task<int> UpdateMemberVerificationStatus(IDbConnection conn, IDbTransaction tx, int memberId)
        {
            string sql = @"Update Member SET IsVerifyEmail = 'Y' Where MembereId = @MembereId AND ISNULL(IsVerifyEmail,'') = 'N' ";
            return await conn.ExecuteAsync(sql, memberId, tx);
        }
    }
}
