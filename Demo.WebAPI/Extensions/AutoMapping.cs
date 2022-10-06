using AutoMapper;

using Demo.Entities;
using Demo.ViewModel;
using System.Collections.Generic;

namespace Demo.WebAPI.Extensions
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {

            AllowNullCollections = true;

            CreateMap<Users, SigninVM>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ReverseMap();

        }
    }
}
