
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using System.Data;
using Dapper;
using BackendSystem.Respository.ResultModel;

namespace BackendSystem.Respository.Implement
{
    public class MemberManagementRespository : IMemberManagementRespository
    {
        private readonly IDbConnection _dbConnection;
        public MemberManagementRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task<IEnumerable<MemberManagementResultModel>> GetAllMember()
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

            var members = await _dbConnection.QueryAsync<MemberManagementResultModel>(sql);
            return members;
        }
        public async Task<int> DeleteMember(MemberManagementCommandModel member)
        {
            string sql = @"UPDATE Member SET IsDeleted = 1 WHERE MemberId = @MemberId AND IsDeleted = 0";

            using var conn = _dbConnection;
            if (conn.State != ConnectionState.Open)
                conn.Open();

            using var tran = conn.BeginTransaction();

            int row;
            try
            {
                row = await conn.ExecuteAsync(sql, new { member.MemberId }, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
            return row;
        }
        public async Task<int> UpdateMember(MemberManagementCommandModel member)
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

            using var conn = _dbConnection;
            if (conn.State != ConnectionState.Open)
                conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                int row = await conn.ExecuteAsync(sql, member, tran);
                tran.Commit();
                return row;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
        public async Task<int> UpdateMemberVerificationStatus(int memberId)
        {
            string sql = @"Update Member SET IsVerifyEmail = 'Y' Where MembereId = @MembereId AND ISNULL(IsVerifyEmail,'') = 'N' ";
            using var conn = _dbConnection;
            int row;
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            using var tran = conn.BeginTransaction();
            try
            {
                row = await _dbConnection.ExecuteAsync(sql, memberId , tran);
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
            return row;
        }
    }
}
