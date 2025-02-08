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

            CreateMap<Pharmacics, PharmacicsDto>();
            CreateMap<PharmacicsDto, Pharmacics>(); // ✅ تحويل DTO إلى كيان


         
            CreateMap<medicineDto, Medicine>()
                .ForMember(dest => dest.ImageMedicine, opt => opt.Ignore()); 

   
            CreateMap<Medicine, medicineDto>()
                .ForMember(dest => dest.ImageMedicine, opt => opt.Ignore());

            CreateMap<PractitionerCreateDto, Practitioner>();
        }
    }


}
