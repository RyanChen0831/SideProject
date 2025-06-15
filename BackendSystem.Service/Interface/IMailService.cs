
using BackendSystem.Service.Dtos;

namespace BackendSystem.Service.Interface
{
    public interface IMailService
    {
        public Task SendRegisterEamil(string toEmail,string name,int Id,string role);
    }
}
