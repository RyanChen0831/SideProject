using AutoMapper;
using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.Interface;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;
using BackendSystem.Service.Security;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Security.Claims;

namespace BackendSystem.Service.Implement
{
    public class MemberService :IMemberService
    {
        private readonly IMemberRespository _memberRespository;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly ITokenService _tokenManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MemberService(IMemberRespository memberRespository, IMapper mapper, IMailService mailService, ITokenService tokenService, IHttpContextAccessor httpContextAccessor)
        {
            _memberRespository = memberRespository;
            _mapper = mapper;
            _mailService = mailService;
            _tokenManager = tokenService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> DeleteMember(MemberInfo member)
        {
            try
            {
                var parm = _mapper.Map<MemberInfo, MemberCondition>(member);
                return await _memberRespository.DeleteMember(parm);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMember()
        {
            try
            {
                var list = await _memberRespository.GetAllMember();
                return _mapper.Map< IEnumerable <MemberCondition> ,IEnumerable<MemberViewModel> > (list);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<MemberViewModel> GetMember(string account, string password)
        {
            try
            {
                var member = await _memberRespository.GetMember(account, password);
                return _mapper.Map<MemberCondition, MemberViewModel>(member);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResultDTO<string>> RegisterMember(MemberInfo member)
        {
            bool register;
            var parm = _mapper.Map<MemberInfo, MemberCondition>(member);
            var checkResult =  await _memberRespository.CheckRegistration(parm);
            if (checkResult.IsSucceed)
            {                
                try
                {
                    register = await _memberRespository.Register(parm);
                    if (register) {
                        var user = await _memberRespository.GetMember(member.Account, member.Password);
                        _mailService.SendRegisterEamil(member.Mail, member.Name, user.UserId, user.Role);
                    }                   
                    return new OperationResultDTO<string>(register, string.Empty);
                }
                catch (Exception error)
                {
                    register = false;
                    return new OperationResultDTO<string>(register, error.Message);
                    throw;
                }
                
            }
            else {
                return new OperationResultDTO<string>(false, checkResult.Message);
            }
        }

        public Task<bool> UpdateMember(MemberInfo member)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateMemberVerificationStatus(int userId)
        {            
             return await _memberRespository.UpdateMemberVerificationStatus(userId);
        }
        
        public async Task<OperationResultDTO<User>> VerifyEmail(string info)
        {

            // 將解碼後的 JSON 字符串轉換為 Token 對象
            var token = JsonConvert.DeserializeObject<Token>(info);

            if (token == null)
            {
                throw new Exception("Invalid token format");
            }

            // 解碼和驗證 Token
            var (user, isValid) = _tokenManager.DecodeToken(token);

            if (!isValid)
            {
                return new OperationResultDTO<User>(false, "驗證失敗");
            }
            var result = await UpdateMemberVerificationStatus(user!.Id);

            return new OperationResultDTO<User>(result, user);
        }

        public int? GetUserId()
        {
            // 從 HttpContext.User.Claims 中尋找使用者ID的 Claim
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }


    }

}
