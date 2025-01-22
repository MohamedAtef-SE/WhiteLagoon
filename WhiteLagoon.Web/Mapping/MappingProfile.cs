using AutoMapper;
using WhiteLagoon.Application.DTOs;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Domain.Entities.Identity;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Villa, VillaViewModel>()
           .ForMember(VVM => VVM.Amenities, O => O.MapFrom(V => V.Amenities.Select(v => v.Name)))
           .ForMember(VVM => VVM.ImageURL, options => options.MapFrom<VillaImageURLResolver>())
           .ReverseMap();
            //          src  , dest
            CreateMap<Amenity, AmenityViewModel>()
                .ForMember(amenityVM => amenityVM.Villa, O => O.MapFrom(amenity => amenity.Villa!.Name));

            //CreateMap<Villa, VillaViewModel>()
            //    .ForMember(VVM => VVM.Amenities, O => O.MapFrom(V => V.Amenities.Select(v => v.Name)));

            CreateMap<AmenityViewModel, Amenity>();

            CreateMap<BookingViewModel, Booking>()
                .ForMember(B => B.CheckInDate, O => O.MapFrom(BVM => BVM.Check_In_Date))
                .ForMember(B => B.CheckOutDate, O => O.MapFrom(BVM => BVM.Check_Out_Date))
                .ReverseMap();

            CreateMap<ApplicationUser, UserDTO>();

            CreateMap<VillaNumber, VillaNumberVM>().ReverseMap();

        }
    }
}
