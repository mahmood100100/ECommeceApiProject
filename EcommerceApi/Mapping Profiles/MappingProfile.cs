using AutoMapper;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Entities.DTO;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Api.Mapping_Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Products, ProductResponseDTO>()
                .ForMember(to => to.Category_Name , from => from.MapFrom(x => x.categories != null ? x.categories.Name : null));
            CreateMap<ProductRequestDTO, Products>()
                .ForMember(to => to.categories , from => from.Ignore());
            CreateMap<Orders, OrderResponseDTO>()
                .ForMember(to => to.UserName, from => from.MapFrom(x => x.localUser != null ? x.localUser.UserName : null));
            CreateMap<LocalUser, LocalUserDTO>();
            CreateMap<IdentityRole, RolesResponseDTO>();
            CreateMap<Categories, CategoriesResponseDTO>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategoryDescription, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.products));


            CreateMap<CategoriesRequestDTO, Categories>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.CategoryDescription));

            CreateMap<OrderDetailsRequestDTO, OrderDetails>()
               .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<OrderDetails, OrderDetailsResponseDTO>()
              .ForMember(dest => dest.product, opt => opt.MapFrom(src => src.products));



        }
    }
}
