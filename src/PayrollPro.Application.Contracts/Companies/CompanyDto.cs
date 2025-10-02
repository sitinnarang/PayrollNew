using System;
using Volo.Abp.Application.Dtos;

namespace PayrollPro.Companies
{
    public class CompanyDto : FullAuditedEntityDto<Guid>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string TaxId { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime EstablishedDate { get; set; }
        public bool IsActive { get; set; }
        public string LogoUrl { get; set; }
        public int EmployeeCount { get; set; }
        
        // Full address for display
        public string FullAddress => $"{Address}, {City}, {State} {ZipCode}, {Country}".Trim(new char[] { ',', ' ' });
        
        // Age in years
        public int CompanyAge => DateTime.Now.Year - EstablishedDate.Year;
    }
}