using AutoMapper;
using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.DTO;

namespace BusinessCardWebAPI.Configrations
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            //BusinessCards
            CreateMap<BusinessCards,CreateBusinessCardsDto>().ReverseMap();
            CreateMap<BusinessCards, UpdateBusinessCardsDto>().ReverseMap();
            CreateMap<BusinessCards, GetBusinessCardsDto>().ReverseMap();
        }
    }
}
