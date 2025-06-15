using AutoMapper;
using BackendSystem.Respository.Dtos;
using BackendSystem.Respository.ResultModel;
using BackendSystem.Service.Dtos;



namespace BackendSystem.Service.ServiceMapping
{
    public class ServiceMapping : Profile
    {
        public ServiceMapping() 
        {
            CreateMap<ProductInfo, ProductCondition>();
            CreateMap<ProductResultModel?, ProductViewModel?>();
            CreateMap<ProductCategoryResultModel,ProductCategoryViewModel>();

            CreateMap<ShoppingCartResultModel, ShoppingCartViewModel>();
            CreateMap<ShoppingCartViewModel, ShoppingCartResultModel>();

            CreateMap<OrderInfo?, OrderCondition?>();
            CreateMap<OrderDetailInfo?, OrderDetailCondition?>();
            CreateMap<OrderStatusInfo?, OrderStatusCondition?>();
            CreateMap<OrderViewModel?, OrderDataModel?>();
            CreateMap<OrderResultModel, OrderDTO>();
            CreateMap<OrderDetailResultModel, OrderDetailDTO>();
            CreateMap<MemberInfo?, MemberCondition?>();
            CreateMap<MemberCondition?, Dtos.MemberResultModel?>();

            CreateMap<Respository.ResultModel.MemberProfileResultModel, MemberViewModel>();

        }
    }
}
