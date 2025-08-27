using AutoMapper;
using Entity.Models;
using Entity.ModelRequest;
using Entity.ModelResponse;

namespace Server.AutoMap;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<Tag, TagResponse>().ReverseMap();
        CreateMap<Tag, TagAdd>().ReverseMap();
        CreateMap<CreateAccount, SystemAccount>().ReverseMap();
        CreateMap<SystemAccount, SystemAccountResponse>().ReverseMap();
        CreateMap<Category, CategoryAdd>().ReverseMap();
        CreateMap<Category, CategoryResponse>().ReverseMap();
        CreateMap<SystemAccount, SystemAccountUserResponse>().ReverseMap();
    }
}