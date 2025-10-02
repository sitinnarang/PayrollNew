using System;
using System.Threading.Tasks;
using PayrollPro.Companies;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace PayrollPro;

public class PayrollProCompanyDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Company, Guid> _companyRepository;
    private readonly IGuidGenerator _guidGenerator;

    public PayrollProCompanyDataSeedContributor(
        IRepository<Company, Guid> companyRepository,
        IGuidGenerator guidGenerator)
    {
        _companyRepository = companyRepository;
        _guidGenerator = guidGenerator;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        // Check if we already have companies seeded
        if (await _companyRepository.GetCountAsync() > 0)
        {
            return;
        }

        await SeedCompaniesAsync();
    }

    private async Task SeedCompaniesAsync()
    {
        var companies = new[]
        {
            new Company
            {
                Name = "TechNova Solutions",
                Code = "TNS001",
                Description = "Leading software development and digital transformation company specializing in enterprise solutions.",
                Address = "1234 Technology Drive, Suite 500",
                City = "San Francisco",
                State = "California",
                ZipCode = "94105",
                Country = "United States",
                Phone = "+1 (555) 123-4567",
                Email = "contact@technova.com",
                Website = "https://www.technova.com",
                TaxId = "12-3456789",
                RegistrationNumber = "TNS2023001",
                EstablishedDate = new DateTime(2018, 3, 15),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1560472354-b33ff0c44a43?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 85
            },
            new Company
            {
                Name = "Global Manufacturing Corp",
                Code = "GMC002",
                Description = "International manufacturing company producing automotive components and industrial machinery.",
                Address = "5678 Industrial Boulevard",
                City = "Detroit",
                State = "Michigan",
                ZipCode = "48201",
                Country = "United States",
                Phone = "+1 (555) 234-5678",
                Email = "info@globalmanufacturing.com",
                Website = "https://www.globalmanufacturing.com",
                TaxId = "23-4567890",
                RegistrationNumber = "GMC2020002",
                EstablishedDate = new DateTime(2015, 7, 22),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1518709268805-4e9042af2176?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 320
            },
            new Company
            {
                Name = "HealthCare Plus",
                Code = "HCP003",
                Description = "Comprehensive healthcare services provider with multiple clinic locations and telemedicine solutions.",
                Address = "9876 Medical Center Way",
                City = "Houston",
                State = "Texas",
                ZipCode = "77030",
                Country = "United States",
                Phone = "+1 (555) 345-6789",
                Email = "contact@healthcareplus.com",
                Website = "https://www.healthcareplus.com",
                TaxId = "34-5678901",
                RegistrationNumber = "HCP2019003",
                EstablishedDate = new DateTime(2016, 11, 8),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1576091160399-112ba8d25d1f?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 150
            },
            new Company
            {
                Name = "EcoGreen Energy",
                Code = "EGE004",
                Description = "Renewable energy solutions company focusing on solar, wind, and sustainable power systems.",
                Address = "2468 Green Valley Road",
                City = "Portland",
                State = "Oregon",
                ZipCode = "97205",
                Country = "United States",
                Phone = "+1 (555) 456-7890",
                Email = "hello@ecogreenenergy.com",
                Website = "https://www.ecogreenenergy.com",
                TaxId = "45-6789012",
                RegistrationNumber = "EGE2021004",
                EstablishedDate = new DateTime(2019, 4, 12),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1473341304170-971dccb5ac1e?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 75
            },
            new Company
            {
                Name = "Financial Advisory Group",
                Code = "FAG005",
                Description = "Premium financial consulting and investment advisory services for individuals and corporations.",
                Address = "1357 Wall Street, Floor 25",
                City = "New York",
                State = "New York",
                ZipCode = "10005",
                Country = "United States",
                Phone = "+1 (555) 567-8901",
                Email = "advisors@financialgroup.com",
                Website = "https://www.financialgroup.com",
                TaxId = "56-7890123",
                RegistrationNumber = "FAG2017005",
                EstablishedDate = new DateTime(2014, 9, 30),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1554224155-6726b3ff858f?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 45
            },
            new Company
            {
                Name = "Creative Design Studio",
                Code = "CDS006",
                Description = "Full-service creative agency specializing in branding, digital design, and marketing campaigns.",
                Address = "7890 Art District Lane",
                City = "Los Angeles",
                State = "California",
                ZipCode = "90028",
                Country = "United States",
                Phone = "+1 (555) 678-9012",
                Email = "creative@designstudio.com",
                Website = "https://www.designstudio.com",
                TaxId = "67-8901234",
                RegistrationNumber = "CDS2020006",
                EstablishedDate = new DateTime(2017, 1, 18),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1561070791-2526d30994b5?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 28
            },
            new Company
            {
                Name = "Logistics Express Inc",
                Code = "LEI007",
                Description = "National logistics and supply chain management company with nationwide distribution network.",
                Address = "4321 Transportation Hub",
                City = "Chicago",
                State = "Illinois",
                ZipCode = "60601",
                Country = "United States",
                Phone = "+1 (555) 789-0123",
                Email = "operations@logisticsexpress.com",
                Website = "https://www.logisticsexpress.com",
                TaxId = "78-9012345",
                RegistrationNumber = "LEI2018007",
                EstablishedDate = new DateTime(2013, 6, 5),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1586528116311-ad8dd3c8310d?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 200
            },
            new Company
            {
                Name = "Restaurant Chain Co",
                Code = "RCC008",
                Description = "Popular restaurant chain serving fresh, locally-sourced American cuisine with 25+ locations.",
                Address = "8642 Culinary Boulevard",
                City = "Nashville",
                State = "Tennessee",
                ZipCode = "37201",
                Country = "United States",
                Phone = "+1 (555) 890-1234",
                Email = "franchise@restaurantchain.com",
                Website = "https://www.restaurantchain.com",
                TaxId = "89-0123456",
                RegistrationNumber = "RCC2016008",
                EstablishedDate = new DateTime(2012, 2, 14),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 480
            },
            new Company
            {
                Name = "Educational Resources Ltd",
                Code = "ERL009",
                Description = "Educational technology company providing online learning platforms and digital curriculum solutions.",
                Address = "9753 Education Park",
                City = "Boston",
                State = "Massachusetts",
                ZipCode = "02101",
                Country = "United States",
                Phone = "+1 (555) 901-2345",
                Email = "support@educationalresources.com",
                Website = "https://www.educationalresources.com",
                TaxId = "90-1234567",
                RegistrationNumber = "ERL2019009",
                EstablishedDate = new DateTime(2020, 8, 20),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1503676260728-1c00da094a0b?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 62
            },
            new Company
            {
                Name = "Construction Masters",
                Code = "CM010",
                Description = "Commercial and residential construction company specializing in sustainable building practices.",
                Address = "1111 Builder's Way",
                City = "Denver",
                State = "Colorado",
                ZipCode = "80202",
                Country = "United States",
                Phone = "+1 (555) 012-3456",
                Email = "projects@constructionmasters.com",
                Website = "https://www.constructionmasters.com",
                TaxId = "01-2345678",
                RegistrationNumber = "CM2015010",
                EstablishedDate = new DateTime(2011, 12, 3),
                IsActive = true,
                LogoUrl = "https://images.unsplash.com/photo-1504307651254-35680f356dfd?w=200&h=200&fit=crop&crop=center",
                EmployeeCount = 125
            }
        };

        foreach (var company in companies)
        {
            await _companyRepository.InsertAsync(company, autoSave: true);
        }
    }
}