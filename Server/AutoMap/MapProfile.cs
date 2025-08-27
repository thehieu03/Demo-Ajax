using AutoMapper;
using Entity.ModelResponse;
using Entity.Models;

namespace Server.AutoMap;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<Tag, TagResponse>().ReverseMap();
        CreateMap<CreateAccount, SystemAccount>().ReverseMap();
        CreateMap<SystemAccount, SystemAccountResponse>().ReverseMap();
        CreateMap<Category, CategoryAdd>().ReverseMap();
        CreateMap<Category, CategoryResponse>().ReverseMap();
    }
}