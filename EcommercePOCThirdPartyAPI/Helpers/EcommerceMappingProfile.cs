using AutoMapper;
using EcommercePOCThirdPartyAPI.DomainModals;

namespace EcommercePOCThirdPartyAPI.Helpers
{
    public class EcommerceMappingProfile : Profile
    {
        public EcommerceMappingProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
        }

    }
}

