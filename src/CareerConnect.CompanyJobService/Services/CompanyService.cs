using System.Net;
using CareerConnect.CompanyJobService.Data;
using CareerConnect.CompanyJobService.Dto;
using CareerConnect.CompanyJobService.Entities;
using CareerConnect.CompanyJobService.Enums;
using CareerConnect.Shared.Clients;
using CareerConnect.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.CompanyJobService.Services;

public class CompanyService(CompanyJobDbContext db, IFileServiceClient fileServiceClient) : ICompanyService
{
    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto dto, long ownerId)
    {
        var company = new Company
        {
            OwnerId = ownerId,
            Name = dto.Name,
            Description = dto.Description,
            Industry = dto.Industry,
            EmployeeRange = dto.EmployeeRange,
            WebsiteUrl = dto.WebsiteUrl
        };

        db.Companies.Add(company);
        await db.SaveChangesAsync();

        return ToDto(company);
    }

    public async Task<string> UploadCompanyFileAsync(IFormFile file, string fileType, long companyId, long ownerId)
    {
        if (!Enum.TryParse<CompanyFileType>(fileType, ignoreCase: true, out var companyFileType))
        {
            throw new ApiException(HttpStatusCode.BadRequest,
                $"Invalid File Type. Only {CompanyFileType.Logo}, {CompanyFileType.Banner} are supported");
        }

        var company = await GetCompanyOrThrowAsync(companyId);
        EnsureOwnerOrAdmin(company, ownerId, isAdmin: false);

        string url;
        try
        {
            await using var stream = file.OpenReadStream();
            url = await fileServiceClient.UploadFileAsync(new Refit.StreamPart(stream, file.FileName, file.ContentType));
        }
        catch (Exception)
        {
            throw new ApiException(HttpStatusCode.InternalServerError, "Error while uploading file");
        }

        var existing = await db.CompanyFiles.FirstOrDefaultAsync(f => f.CompanyId == companyId && f.Type == companyFileType);
        if (existing is not null)
        {
            existing.Link = url;
        }
        else
        {
            db.CompanyFiles.Add(new CompanyFiles { CompanyId = companyId, Type = companyFileType, Link = url });
        }

        await db.SaveChangesAsync();

        return url;
    }

    public async Task<List<CompanyDto>> GetAllCompaniesAsync()
    {
        var companies = await db.Companies.Include(c => c.Files).ToListAsync();
        return companies.Select(ToDto).ToList();
    }

    public async Task<CompanyDto> GetCompanyByIdAsync(long id)
    {
        var company = await db.Companies.Include(c => c.Files).FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"company with id {id} was not found");

        return ToDto(company);
    }

    public async Task<CompanyDetailedDto> GetCompanyDetailedAsync(long id)
    {
        var company = await db.Companies
            .Include(c => c.Files)
            .Include(c => c.Locations)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"company with id {id} was not found");

        var numJobs = await db.Jobs.CountAsync(j => j.CompanyId == id);

        var dto = ToDto(company);
        return new CompanyDetailedDto
        {
            Id = dto.Id,
            OwnerId = dto.OwnerId,
            Name = dto.Name,
            Description = dto.Description,
            Industry = dto.Industry,
            EmployeeRange = dto.EmployeeRange,
            WebsiteUrl = dto.WebsiteUrl,
            Logo = dto.Logo,
            Banner = dto.Banner,
            NumJobs = numJobs,
            Locations = company.Locations.Select(ToLocationDto).ToList()
        };
    }

    public async Task<CompanyDto> UpdateCompanyAsync(long id, UpdateCompanyDto dto, long ownerId, bool isAdmin)
    {
        var company = await GetCompanyOrThrowAsync(id);
        EnsureOwnerOrAdmin(company, ownerId, isAdmin);

        if (dto.Name is not null) company.Name = dto.Name;
        if (dto.Description is not null) company.Description = dto.Description;
        if (dto.Industry is not null) company.Industry = dto.Industry;
        if (dto.EmployeeRange is not null) company.EmployeeRange = dto.EmployeeRange.Value;
        if (dto.WebsiteUrl is not null) company.WebsiteUrl = dto.WebsiteUrl;

        await db.SaveChangesAsync();

        return ToDto(company);
    }

    public async Task DeleteCompanyAsync(long id, long ownerId, bool isAdmin)
    {
        var company = await GetCompanyOrThrowAsync(id);
        EnsureOwnerOrAdmin(company, ownerId, isAdmin);

        db.Companies.Remove(company);
        await db.SaveChangesAsync();
    }

    public async Task<CompanyLocationDto> AddLocationAsync(CreateCompanyLocationDto dto, long ownerId)
    {
        var company = await GetCompanyOrThrowAsync(dto.CompanyId);
        EnsureOwnerOrAdmin(company, ownerId, isAdmin: false);

        var location = new CompanyLocation
        {
            CompanyId = dto.CompanyId,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            IsRemote = dto.IsRemote
        };

        db.CompanyLocations.Add(location);
        await db.SaveChangesAsync();

        return ToLocationDto(location);
    }

    public async Task<CompanyLocationDto> UpdateLocationAsync(long id, UpdateCompanyLocationDto dto, long ownerId, bool isAdmin)
    {
        var location = await db.CompanyLocations.Include(l => l.Company).FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"company location with id {id} was not found");

        EnsureOwnerOrAdmin(location.Company, ownerId, isAdmin);

        if (dto.Address is not null) location.Address = dto.Address;
        if (dto.City is not null) location.City = dto.City;
        if (dto.Country is not null) location.Country = dto.Country;
        if (dto.IsRemote is not null) location.IsRemote = dto.IsRemote.Value;

        await db.SaveChangesAsync();

        return ToLocationDto(location);
    }

    public async Task DeleteLocationAsync(long id, long ownerId, bool isAdmin)
    {
        var location = await db.CompanyLocations.Include(l => l.Company).FirstOrDefaultAsync(l => l.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"company location with id {id} was not found");

        EnsureOwnerOrAdmin(location.Company, ownerId, isAdmin);

        db.CompanyLocations.Remove(location);
        await db.SaveChangesAsync();
    }

    private async Task<Company> GetCompanyOrThrowAsync(long id) =>
        await db.Companies.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"company with id {id} was not found");

    private static void EnsureOwnerOrAdmin(Company company, long userId, bool isAdmin)
    {
        if (company.OwnerId != userId && !isAdmin)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You do not manage this company");
        }
    }

    private static CompanyDto ToDto(Company company) => new()
    {
        Id = company.Id,
        OwnerId = company.OwnerId,
        Name = company.Name,
        Description = company.Description,
        Industry = company.Industry,
        EmployeeRange = company.EmployeeRange,
        WebsiteUrl = company.WebsiteUrl,
        Logo = company.Files?.FirstOrDefault(f => f.Type == CompanyFileType.Logo)?.Link,
        Banner = company.Files?.FirstOrDefault(f => f.Type == CompanyFileType.Banner)?.Link
    };

    private static CompanyLocationDto ToLocationDto(CompanyLocation location) => new()
    {
        Id = location.Id,
        CompanyId = location.CompanyId,
        Address = location.Address,
        City = location.City,
        Country = location.Country,
        IsRemote = location.IsRemote
    };
}
