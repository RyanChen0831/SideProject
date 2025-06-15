using AutoMapper;
using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.Interface;
using BackendSystem.Respository.ResultModel;
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

        public async Task<IEnumerable<Dtos.MemberResultModel>> GetAllMember()
        {
            try
            {
                var list = await _memberRespository.GetAllMember();
                return _mapper.Map<IEnumerable<MemberCondition>, IEnumerable<Dtos.MemberResultModel>> (list);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Dtos.MemberResultModel> GetMember(string account, string password)
        {
            try
            {
                var member = await _memberRespository.GetMember(account, password);
                return _mapper.Map<MemberCondition, Dtos.MemberResultModel>(member);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<OperationResultDTO<string>> RegisterMember(MemberInfo member)
        {
            try
            {
                var parm = _mapper.Map<MemberCondition>(member);
                var checkResult = await _memberRespository.CheckRegistration(parm);
                if (checkResult.IsSucceed)
                {
                    if (await _memberRespository.Register(parm))
                    {
                        var user = await _memberRespository.GetMember(member.Account, member.Password);
                        await _mailService.SendRegisterEamil(member.Mail, member.Name, user.MemberId, user.Role);
                    }
                    return new OperationResultDTO<string>(true, string.Empty);
                }
                else
                {
                    return new OperationResultDTO<string>(false, checkResult.Message);
                }
            }
            catch (Exception error)
            {
                return new OperationResultDTO<string>(false, $"註冊時發生錯誤: {error.Message}");
            }
        }

        public Task<bool> UpdateMember(MemberInfo member)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateMemberVerificationStatus(int memberId)
        {            
             return await _memberRespository.UpdateMemberVerificationStatus(memberId);
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

        public int? GetMemberId()
        {
            // 從 HttpContext.User.Claims 中尋找使用者ID的 Claim
            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int memberId))
            {
                return memberId;
            }

            return null;
        }

        public async Task<MemberViewModel> GetMember(int memberId)
        {
            try
            {
                var member = await _memberRespository.GetMember(memberId);
                return _mapper.Map<Respository.ResultModel.MemberProfileResultModel, MemberViewModel>(member);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

}
