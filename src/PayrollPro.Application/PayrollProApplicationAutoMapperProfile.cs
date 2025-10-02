using System;
using AutoMapper;
using PayrollPro.Companies;
using PayrollPro.Employees;
using PayrollPro.Payrolls;
using PayrollPro.Timesheets;

namespace PayrollPro;

public class PayrollProApplicationAutoMapperProfile : Profile
{
    public PayrollProApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        // Company mappings
        CreateMap<Company, CompanyDto>();
        CreateMap<CreateUpdateCompanyDto, Company>();

        // Employee mappings
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company != null ? src.Company.Name : ""))
            .ForMember(dest => dest.CompanyCode, opt => opt.MapFrom(src => src.Company != null ? src.Company.Code : ""));
        CreateMap<CreateUpdateEmployeeDto, Employee>();

        // PayrollRecord mappings
        CreateMap<PayrollRecord, PayrollRecordDto>();
        CreateMap<CreateUpdatePayrollRecordDto, PayrollRecord>();

        // Timesheet mappings
        CreateMap<Timesheet, TimesheetDto>();
        CreateMap<CreateUpdateTimesheetDto, Timesheet>();
    }
}
