using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModel;
using Dapper;
using System.Data;

namespace BackendSystem.Respository.Implement
{
    public class MemberRespository: IMemberRespository
    {
        private readonly IDbConnection _dbConnection;
        public MemberRespository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<RegisterOperationResultModel> CheckRegistration(MemberCommandModel member)
        {
            string str = @"SELECT Account,Phone,Mail
                           FROM Member
                           Where Account=@Account OR Phone=@Phone OR Mail=@Mail ";
            var parm = new DynamicParameters();
            parm.Add("Account", member.Account);
            parm.Add("Phone", member.Phone);
            parm.Add("Mail", member.Mail);

            var result =  await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(str,parm);

            if (result == null)
            {
                return new RegisterOperationResultModel(true, string.Empty);
            }
            if (result.Account == member.Account) {
                return new RegisterOperationResultModel { IsSucceed = false, Message = "Account has already been registered." };
            }
            if (result.Phone == member.Phone) {
                return new RegisterOperationResultModel { IsSucceed = false, Message = "Phone has already been registered." };
            }
            if (result.Mail == member.Mail)
            {
                return new RegisterOperationResultModel { IsSucceed = false, Message = "Mail has already been registered." };
            }
            return new RegisterOperationResultModel(true, string.Empty);
        }

        public async Task<MemberCommandModel> GetMember(string account, string password)
        {
            string sql = @" SELECT * FROM Member WHERE Account = @Account AND Password = @Password ";
            var parm = new DynamicParameters();
            parm.Add("Account", account,DbType.String);
            parm.Add("Password", password, DbType.String);
            var members = await _dbConnection.QueryFirstOrDefaultAsync<MemberCommandModel>(sql, parm);
            return members;
        }

        public async Task<MemberProfileResultModel> GetMember(int memberId)
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
            var member = await _dbConnection.QueryFirstOrDefaultAsync<MemberProfileResultModel>(sql, parm);

            return member;
        }

        public async Task<bool> Register(MemberCommandModel member)
        {
            string sql = @"INSERT INTO Member (Name, Account, Password, Gender, Birthday, Phone, Address, Mail, Role,IsVerifyEmail) 
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
    }
}
