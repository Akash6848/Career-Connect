using CareerConnect.CompanyJobService.Dto;
using Microsoft.AspNetCore.Http;

namespace CareerConnect.CompanyJobService.Services;

public interface ICompanyService
{
    Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto dto, long ownerId);
    Task<string> UploadCompanyFileAsync(IFormFile file, string fileType, long companyId, long ownerId);
    Task<List<CompanyDto>> GetAllCompaniesAsync();
    Task<CompanyDto> GetCompanyByIdAsync(long id);
    Task<CompanyDetailedDto> GetCompanyDetailedAsync(long id);
    Task<CompanyDto> UpdateCompanyAsync(long id, UpdateCompanyDto dto, long ownerId, bool isAdmin);
    Task DeleteCompanyAsync(long id, long ownerId, bool isAdmin);

    Task<CompanyLocationDto> AddLocationAsync(CreateCompanyLocationDto dto, long ownerId);
    Task<CompanyLocationDto> UpdateLocationAsync(long id, UpdateCompanyLocationDto dto, long ownerId, bool isAdmin);
    Task DeleteLocationAsync(long id, long ownerId, bool isAdmin);
}
