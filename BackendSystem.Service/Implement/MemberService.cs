using AutoMapper;
using BackendSystem.Common.Dtos;
using BackendSystem.Common.Interface;
using BackendSystem.Respository.CommandModels;
using BackendSystem.Respository.Interface;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;
using BackendSystem.Service.QueryModels;
using BackendSystem.Service.ResultModels;
using FluentValidation;


namespace BackendSystem.Service.Implement
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRespository _memberRespository;
        private readonly IMemberManagementRespository _memberMangementRespository;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IJWTHelper _jWTHelper;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IValidator<MemberRegisterModel> _validator;
        public MemberService(
            IMemberRespository memberRespository,
            IMapper mapper,
            IMailService mailService,
            IJWTHelper jWTHelper, 
            IDbConnectionFactory connectionFactory, 
            IMemberManagementRespository memberManagementRespository, 
            IPasswordHasher passwordHasher, 
            IValidator<MemberRegisterModel> validator)
        {
            _memberRespository = memberRespository;
            _mapper = mapper;
            _mailService = mailService;
            _jWTHelper = jWTHelper;
            _dbConnectionFactory = connectionFactory;
            _memberMangementRespository = memberManagementRespository;
            _passwordHasher = passwordHasher;
            _validator = validator;
        }

        public async Task<MemberResultModel> ValidateLoginAsync(string account, string password)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();
            var member = await _memberRespository.GetMemberByAccount(conn, account);
            if (member == null || !_passwordHasher.Verify(password, member.PasswordHash))
            {
                throw new UnauthorizedAccessException("帳號或密碼錯誤錯誤");
            }
            return _mapper.Map<MemberResultModel>(member);
        }

        public async Task<OperationResultDTO<string>> RegisterMember(MemberRegisterModel member)
        {
            // 1. 驗證資料
            var validation = await _validator.ValidateAsync(member);
            if (!validation.IsValid)
            {
                var errorMsg = string.Join("；", validation.Errors.Select(e => e.ErrorMessage));
                return new OperationResultDTO<string>(false, errorMsg);
            }

            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {

                var command = _mapper.Map<MemberCommandModel>(member);

                // 2. 密碼雜湊
                command.PasswordHash = _passwordHasher.Hash(member.Password);

                // 3. 新增會員
                var user = await _memberRespository.CreateMember(conn, tx, command);

                // 4. Commit 資料庫交易
                tx.Commit();

                // 5. 發送驗證信
                await _mailService.SendRegisterEamil(member.Mail, member.Name, user.MemberId, user.Role);

                return new OperationResultDTO<string>(true, "註冊成功，請完成信箱驗證，啟用帳號");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return new OperationResultDTO<string>(false, "註冊失敗，請稍後再試");
            }
        }

        public async Task<OperationResultDTO<User>> VerifyEmail(string info)
        {
            //驗證Token
            var user = _jWTHelper.ValidateToken(info);
            if (user == null)
            {
                return new OperationResultDTO<User>(false, "Invalid token");
            }
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();
            var response = _mapper.Map<User>(user);
            try
            {
                var result = await _memberMangementRespository.UpdateMemberVerificationStatus(conn, tx, response.Id) > 0;
                tx.Commit();
                return new OperationResultDTO<User>(result, response);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                return new OperationResultDTO<User>(false, "信件驗證失敗");
                throw;
            }

        }

        public async Task<MemberViewModel?> GetMember(int memberId)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();
            try
            {
                var result = await _memberRespository.GetMember(conn, memberId);
                if (result == null) 
                    return null;
                return _mapper.Map<MemberViewModel>(result);
            }
            catch (Exception)
            {
                throw new ApplicationException("取得會員資料時發生錯誤，請聯絡系統管理員。");
            }
        }
    }

}
