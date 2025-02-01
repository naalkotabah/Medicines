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
            CreateMap<Pharmacics, PharmacicsDto>();
            CreateMap<PharmacicsDto, Pharmacics>(); // ✅ تحويل DTO إلى كيان


            CreateMap<Medicine, medicineDto>();
            CreateMap<medicineDto, Medicine>();
        }
    }


}
