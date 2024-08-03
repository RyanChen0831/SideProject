using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.Interface;
using Dapper;
using System.Data;
using System.Diagnostics.Metrics;

namespace BackendSystem.Respository.Implement
{
    public class MemberRespository: IMemberRespository
    {
        private readonly IDbConnection _dbConnection;
        public MemberRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<RegisterOperationResult> CheckRegistration(MemberCondition member)
        {
            string str = @"SELECT Account,Phone,Mail
                           FROM [User]
                           Where Account=@Account OR Phone=@Phone OR Mail=@Mail ";
            var parm = new DynamicParameters();
            parm.Add("Account", member.Account);
            parm.Add("Phone", member.Phone);
            parm.Add("Mail", member.Mail);

            var result =  await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(str,parm);

            if (result == null)
            {
                return new RegisterOperationResult(true, string.Empty);
            }
            if (result.Account == member.Account) {
                return new RegisterOperationResult { IsSucceed = false, Message = "Account has already been registered." };
            }
            if (result.Phone == member.Phone) {
                return new RegisterOperationResult { IsSucceed = false, Message = "Phone has already been registered." };
            }
            if (result.Mail == member.Mail)
            {
                return new RegisterOperationResult { IsSucceed = false, Message = "Mail has already been registered." };
            }
            return new RegisterOperationResult(true, string.Empty);
        }

        public Task<bool> DeleteMember(MemberCondition member)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MemberCondition>> GetAllMember()
        {
            string sql = @" SELECT * FROM [User] WHERE Role = 'User' ";
            var members = await _dbConnection.QueryAsync<MemberCondition>(sql);
            return members;
        }

        public async Task<MemberCondition> GetMember(string account, string password)
        {
            string sql = @" SELECT * FROM [User] WHERE Account = @Account AND Password = @Password ";
            var parm = new DynamicParameters();
            parm.Add("Account", account,DbType.String);
            parm.Add("Password", password, DbType.String);
            var members = await _dbConnection.QueryFirstOrDefaultAsync<MemberCondition>(sql, parm);
            return members;
        }

        public async Task<bool> Register(MemberCondition member)
        {
            string sql = @"INSERT INTO [User] (Name, Account, Password, Gender, Birthday, Phone, Address, Mail, Role,IsVerifyEmail) 
                   VALUES (@Name, @Account, @Password, @Gender, @Birthday, @Phone, @Address, @Mail, @Role,'N')";
            var parm = new DynamicParameters();
            parm.AddDynamicParams(member);
            try
            {
                await _dbConnection.ExecuteAsync(sql, parm);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<bool> UpdateMember(MemberCondition member)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateMemberVerificationStatus(int userId)
        {
            string sql = @"Update [User] SET IsVerifyEmail = 'Y' Where UserID = @UserId ";
            var parm = new DynamicParameters();
            parm.Add("UserId",userId);
            try
            {
                await _dbConnection.ExecuteAsync(sql, parm);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
