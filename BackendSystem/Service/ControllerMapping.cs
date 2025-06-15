using AutoMapper;
using BackendSystem.Dtos;
using BackendSystem.Service.Dtos;

namespace BackendSystem.Service
{
    public class ControllerMapping:Profile
    {
        public ControllerMapping()
        {

            CreateMap<RegisterDTO, MemberInfo>();
            CreateMap<NewProductParameter, ProductInfo>();

        }

    }
}
