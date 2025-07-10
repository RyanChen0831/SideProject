using AutoMapper;
using BackendSystem.Respository.Interface;
using BackendSystem.Service.QueryModels;
using BackendSystem.Service.Interface;
using BackendSystem.Service.ResultModels;
using Microsoft.Extensions.Logging;
using BackendSystem.Respository.CommandModels;

namespace BackendSystem.Service.Implement
{
    public class MemberManagementService : IMemberManagementService
    {
        private readonly IMemberManagementRespository _memberManagementRespository;
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public MemberManagementService(IMemberManagementRespository memberManagementRespository, IMapper mapper, IDbConnectionFactory connectionFactory, ILogger logger)
        {

            _memberManagementRespository = memberManagementRespository;
            _dbConnectionFactory = connectionFactory;
            _mapper = mapper;
            _logger = logger;

        }

        public async Task<bool> DeleteMemberAsync(int memberId)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                bool success = await _memberManagementRespository.DeleteMember(conn, tx, memberId) > 0;
                if (!success)
                {
                    tx.Rollback();
                    return false;
                }
                tx.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                _logger.LogError(ex, "刪除會員失敗，MemberId: {Id}", memberId);
                return false;
            }
        }

        public async Task<IEnumerable<MemberResultModel>> GetAllMembersAsync()
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();
            var res = await _memberManagementRespository.GetAllMember(conn);
            var members = _mapper.Map<IEnumerable<MemberResultModel>>(res);
            return members;
        }

        public async Task<bool> UpdateMemberAsync(MemberQueryModel member)
        {
            using var conn = _dbConnectionFactory.CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                var commad = _mapper.Map<MemberManagementCommandModel>(member);
                var success = await _memberManagementRespository.UpdateMember(conn, tx, commad) > 0;
                if (!success) { tx.Rollback(); return false; }
                tx.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                _logger.LogError(ex, "更新會員失敗，MemberName: {Name}", member.Name);
                return false;
                throw;
            }
        }
    }
}
