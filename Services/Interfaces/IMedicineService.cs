﻿namespace Medicines.Services.Interfaces
{
    using Medicines.Data.dto;

    public interface IMedicineService
    {
        Task<object> GetAllAsync(int pageNumber, int pageSize , int Pharmacice);
        Task<(bool Success, string Message, object? Data)> AddAsync(medicineDto dto);
        Task<(bool Success, string Message, object? Data)> UpdateAsync(int id, medicineDto dto);
        Task<(bool Success, string Message, object? Data)> DeleteAsync(int id);
        Task<List<MedicinforserchDto>> SerchForMedicineHome(string name);
    }

}
