using Medicines.Data.Models;

namespace Medicines.Repositories.Interfaces
{
    public interface IPractitionerRepository
    {
        Task<Practitioner?> GetByLoginAsync(string name, string password);
        Task<List<Medicine>> SearchMedicinesForPractitioner(int practitionerId, string search);

        Task<List<Practitioner>> GetAllWithPharmacyAsync();
        Task<List<Practitioner>> GetForSelectAsync();
        Task<Practitioner?> GetPharmacyByPractitionerIdAsync(int id);
        Task<Practitioner?> GetByIdAsync(int id);
        Task<bool> IsPractitionerLinkedToPharmacy(int id);
        Task AddAsync(Practitioner practitioner);
        Task UpdateAsync(Practitioner practitioner);
        Task DeleteAsync(Practitioner practitioner);
    }

}
