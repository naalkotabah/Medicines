namespace Medicines.Mapping
{

    using AutoMapper;
    using Medicines.Data.dto;
    using Medicines.Data.Models;
    public class Mappings : Profile
    {
        public Mappings()
        {
            CreateMap<Users, UserDto>();


            CreateMap<Users, LoginDto>();

            CreateMap<PharmacicsDto, Pharmacics>()
      .ForMember(dest => dest.OpenTime,
          opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.OpenTime) ? TimeSpan.Zero : TimeSpan.Parse(src.OpenTime)))
      .ForMember(dest => dest.CloseTime,
          opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.CloseTime) ? TimeSpan.Zero : TimeSpan.Parse(src.CloseTime)));





            CreateMap<medicineDto, Medicine>()
                .ForMember(dest => dest.ImageMedicine, opt => opt.Ignore()); 

   
            CreateMap<Medicine, medicineDto>()
                .ForMember(dest => dest.ImageMedicine, opt => opt.Ignore());

            CreateMap<PractitionerCreateDto, Practitioner>();
        }
    }


}
