namespace Medicines.Services.Interfaces
{
    using Medicines.Data.dto;

    public interface IPractitionerService
    {
        Task<object?> LoginAsync(Login_Practitioner dto);
        Task<List<object>> GetAllAsync();
        Task<List<object>> GetForSelectAsync();
        Task<List<SearchMedicinesDto>> SearchMedicinesForPractitioner(int practitionerId, string search);
        Task<object?> GetMyPharmacyAsync(int id);
        Task<(bool, string, object?)> AddAsync(PractitionerCreateDto dto);
        Task<(bool, string, object?)> UpdateAsync(int id, PractitionerCreateDto dto);
        Task<(bool, string, object?)> DeleteAsync(int id);
    }

}
