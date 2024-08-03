using AutoMapper;
using BackendSystem.Respository.Dtos;
using BackendSystem.Service.Dtos;



namespace BackendSystem.Service.ServiceMapping
{
    public class ServiceMapping:Profile
    {
        public ServiceMapping() 
        {
            CreateMap<ProductInfo, ProductCondition>();
            CreateMap<ProductDataModel?, ProductViewModel?>();
            CreateMap<ShoppingCartDataModel?, ShoppingCartViewModel?>();
            
            CreateMap<OrderInfo?, OrderCondition?>();
            CreateMap<OrderDetailInfo?, OrderDetailCondition?>();
            CreateMap<OrderStatusInfo?, OrderStatusCondition?>();
            CreateMap<OrderViewModel?, OrderDataModel?>();

            CreateMap<MemberInfo?, MemberCondition?>();
            CreateMap<MemberCondition?, MemberViewModel?>();
        }
    }
}
