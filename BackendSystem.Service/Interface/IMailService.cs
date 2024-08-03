
namespace BackendSystem.Service.Interface
{
    public interface IMailService
    {
        public void SendRegisterEamil(string toEmail,string name,int Id,string role);
    }
}
