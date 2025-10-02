using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using PayrollPro.Employees;
using PayrollPro.Companies;

namespace PayrollPro
{
    public class PayrollProEmployeeDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Employee, Guid> _employeeRepository;
        private readonly IRepository<Company, Guid> _companyRepository;
        private readonly IGuidGenerator _guidGenerator;

        public PayrollProEmployeeDataSeedContributor(
            IRepository<Employee, Guid> employeeRepository,
            IRepository<Company, Guid> companyRepository,
            IGuidGenerator guidGenerator)
        {
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Check if we already have employees seeded
            if (await _employeeRepository.GetCountAsync() > 0)
            {
                return;
            }

            var companies = await _companyRepository.GetListAsync();
            
            foreach (var company in companies)
            {
                await SeedEmployeesForCompanyAsync(company);
            }
        }

        private async Task SeedEmployeesForCompanyAsync(Company company)
        {
            var employees = GetEmployeesForCompany(company);
            
            foreach (var employee in employees)
            {
                await _employeeRepository.InsertAsync(employee, autoSave: true);
            }
        }

        private Employee[] GetEmployeesForCompany(Company company)
        {
            switch (company.Name)
            {
                case "TechNova Solutions":
                    return new[]
                    {
                        CreateEmployee(company.Id, "John", "Smith", "john.smith@technova.com", "+1-555-0101", "TN001", "Engineering", "Senior Software Engineer", 95000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Sarah", "Johnson", "sarah.johnson@technova.com", "+1-555-0102", "TN002", "Engineering", "Full Stack Developer", 82000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Michael", "Davis", "michael.davis@technova.com", "+1-555-0103", "TN003", "Engineering", "DevOps Engineer", 88000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Emily", "Wilson", "emily.wilson@technova.com", "+1-555-0104", "TN004", "Product", "Product Manager", 105000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-400)),
                        CreateEmployee(company.Id, "David", "Brown", "david.brown@technova.com", "+1-555-0105", "TN005", "Design", "UX Designer", 78000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Lisa", "Taylor", "lisa.taylor@technova.com", "+1-555-0106", "TN006", "Engineering", "Frontend Developer", 75000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-90)),
                        CreateEmployee(company.Id, "James", "Anderson", "james.anderson@technova.com", "+1-555-0107", "TN007", "Engineering", "Backend Developer", 85000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-365)),
                        CreateEmployee(company.Id, "Jennifer", "Martinez", "jennifer.martinez@technova.com", "+1-555-0108", "TN008", "QA", "QA Engineer", 68000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Robert", "Garcia", "robert.garcia@technova.com", "+1-555-0109", "TN009", "Engineering", "Tech Lead", 115000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-500)),
                        CreateEmployee(company.Id, "Amanda", "Rodriguez", "amanda.rodriguez@technova.com", "+1-555-0110", "TN010", "Marketing", "Digital Marketing Specialist", 55000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150))
                    };

                case "Global Manufacturing Corp":
                    return new[]
                    {
                        CreateEmployee(company.Id, "William", "Thompson", "william.thompson@globalmanuf.com", "+1-555-0201", "GM001", "Operations", "Operations Manager", 92000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-400)),
                        CreateEmployee(company.Id, "Jessica", "White", "jessica.white@globalmanuf.com", "+1-555-0202", "GM002", "Engineering", "Manufacturing Engineer", 78000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Christopher", "Harris", "christopher.harris@globalmanuf.com", "+1-555-0203", "GM003", "Quality", "Quality Control Supervisor", 65000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Michelle", "Clark", "michelle.clark@globalmanuf.com", "+1-555-0204", "GM004", "Logistics", "Supply Chain Coordinator", 58000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Daniel", "Lewis", "daniel.lewis@globalmanuf.com", "+1-555-0205", "GM005", "Maintenance", "Maintenance Technician", 52000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-365)),
                        CreateEmployee(company.Id, "Nicole", "Walker", "nicole.walker@globalmanuf.com", "+1-555-0206", "GM006", "Safety", "Safety Officer", 62000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Matthew", "Hall", "matthew.hall@globalmanuf.com", "+1-555-0207", "GM007", "Production", "Production Supervisor", 68000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-450)),
                        CreateEmployee(company.Id, "Ashley", "Allen", "ashley.allen@globalmanuf.com", "+1-555-0208", "GM008", "HR", "HR Specialist", 55000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Anthony", "Young", "anthony.young@globalmanuf.com", "+1-555-0209", "GM009", "Finance", "Cost Analyst", 60000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Stephanie", "King", "stephanie.king@globalmanuf.com", "+1-555-0210", "GM010", "Engineering", "Process Engineer", 75000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150))
                    };

                case "HealthCare Plus":
                    return new[]
                    {
                        CreateEmployee(company.Id, "Dr. Richard", "Wright", "richard.wright@healthcareplus.com", "+1-555-0301", "HC001", "Medical", "Chief Medical Officer", 220000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-800)),
                        CreateEmployee(company.Id, "Dr. Maria", "Lopez", "maria.lopez@healthcareplus.com", "+1-555-0302", "HC002", "Medical", "Cardiologist", 185000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-600)),
                        CreateEmployee(company.Id, "Susan", "Hill", "susan.hill@healthcareplus.com", "+1-555-0303", "HC003", "Nursing", "Head Nurse", 78000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-500)),
                        CreateEmployee(company.Id, "Thomas", "Scott", "thomas.scott@healthcareplus.com", "+1-555-0304", "HC004", "Administration", "Healthcare Administrator", 85000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-400)),
                        CreateEmployee(company.Id, "Karen", "Green", "karen.green@healthcareplus.com", "+1-555-0305", "HC005", "Pharmacy", "Clinical Pharmacist", 95000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Brian", "Adams", "brian.adams@healthcareplus.com", "+1-555-0306", "HC006", "IT", "Healthcare IT Specialist", 72000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Laura", "Baker", "laura.baker@healthcareplus.com", "+1-555-0307", "HC007", "Medical", "Physician Assistant", 105000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Kevin", "Gonzalez", "kevin.gonzalez@healthcareplus.com", "+1-555-0308", "HC008", "Lab", "Lab Technician", 48000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Rachel", "Nelson", "rachel.nelson@healthcareplus.com", "+1-555-0309", "HC009", "Billing", "Medical Billing Specialist", 45000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150)),
                        CreateEmployee(company.Id, "Steven", "Carter", "steven.carter@healthcareplus.com", "+1-555-0310", "HC010", "Security", "Security Supervisor", 52000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120))
                    };

                case "EcoGreen Energy":
                    return new[]
                    {
                        CreateEmployee(company.Id, "Mark", "Mitchell", "mark.mitchell@ecogreen.com", "+1-555-0401", "EG001", "Engineering", "Renewable Energy Engineer", 88000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Angela", "Perez", "angela.perez@ecogreen.com", "+1-555-0402", "EG002", "Project Management", "Project Manager", 95000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-400)),
                        CreateEmployee(company.Id, "Gregory", "Roberts", "gregory.roberts@ecogreen.com", "+1-555-0403", "EG003", "Operations", "Site Operations Manager", 82000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Christina", "Turner", "christina.turner@ecogreen.com", "+1-555-0404", "EG004", "Environmental", "Environmental Compliance Officer", 75000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Jason", "Phillips", "jason.phillips@ecogreen.com", "+1-555-0405", "EG005", "Maintenance", "Turbine Technician", 65000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Melissa", "Campbell", "melissa.campbell@ecogreen.com", "+1-555-0406", "EG006", "Sales", "Business Development Manager", 78000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Ryan", "Parker", "ryan.parker@ecogreen.com", "+1-555-0407", "EG007", "Engineering", "Electrical Engineer", 85000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-365)),
                        CreateEmployee(company.Id, "Heather", "Evans", "heather.evans@ecogreen.com", "+1-555-0408", "EG008", "Finance", "Financial Analyst", 68000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150)),
                        CreateEmployee(company.Id, "Joshua", "Edwards", "joshua.edwards@ecogreen.com", "+1-555-0409", "EG009", "Safety", "Safety Coordinator", 58000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Rebecca", "Collins", "rebecca.collins@ecogreen.com", "+1-555-0410", "EG010", "HR", "HR Generalist", 62000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250))
                    };

                case "Financial Advisory Group":
                    return new[]
                    {
                        CreateEmployee(company.Id, "Charles", "Stewart", "charles.stewart@financialadvisory.com", "+1-555-0501", "FA001", "Advisory", "Senior Financial Advisor", 125000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-600)),
                        CreateEmployee(company.Id, "Patricia", "Sanchez", "patricia.sanchez@financialadvisory.com", "+1-555-0502", "FA002", "Investment", "Investment Analyst", 85000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Kenneth", "Morris", "kenneth.morris@financialadvisory.com", "+1-555-0503", "FA003", "Compliance", "Compliance Officer", 95000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-400)),
                        CreateEmployee(company.Id, "Donna", "Rogers", "donna.rogers@financialadvisory.com", "+1-555-0504", "FA004", "Operations", "Operations Specialist", 58000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Jeffrey", "Reed", "jeffrey.reed@financialadvisory.com", "+1-555-0505", "FA005", "Research", "Research Analyst", 78000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Elizabeth", "Cook", "elizabeth.cook@financialadvisory.com", "+1-555-0506", "FA006", "Client Services", "Client Relationship Manager", 72000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Frank", "Morgan", "frank.morgan@financialadvisory.com", "+1-555-0507", "FA007", "Portfolio", "Portfolio Manager", 115000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-500)),
                        CreateEmployee(company.Id, "Carol", "Bell", "carol.bell@financialadvisory.com", "+1-555-0508", "FA008", "Marketing", "Marketing Coordinator", 52000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150)),
                        CreateEmployee(company.Id, "Raymond", "Murphy", "raymond.murphy@financialadvisory.com", "+1-555-0509", "FA009", "IT", "IT Support Specialist", 55000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Sandra", "Bailey", "sandra.bailey@financialadvisory.com", "+1-555-0510", "FA010", "Administration", "Administrative Assistant", 42000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300))
                    };

                case "Creative Design Studio":
                    return new[]
                    {
                        CreateEmployee(company.Id, "Benjamin", "Rivera", "benjamin.rivera@creativedesign.com", "+1-555-0601", "CD001", "Design", "Creative Director", 98000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-500)),
                        CreateEmployee(company.Id, "Nancy", "Cooper", "nancy.cooper@creativedesign.com", "+1-555-0602", "CD002", "Design", "Senior Graphic Designer", 72000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Jonathan", "Richardson", "jonathan.richardson@creativedesign.com", "+1-555-0603", "CD003", "Design", "Web Designer", 65000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Kimberly", "Cox", "kimberly.cox@creativedesign.com", "+1-555-0604", "CD004", "Design", "UX/UI Designer", 78000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Scott", "Ward", "scott.ward@creativedesign.com", "+1-555-0605", "CD005", "Development", "Frontend Developer", 68000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Linda", "Torres", "linda.torres@creativedesign.com", "+1-555-0606", "CD006", "Project Management", "Project Coordinator", 58000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150)),
                        CreateEmployee(company.Id, "Peter", "Peterson", "peter.peterson@creativedesign.com", "+1-555-0607", "CD007", "Design", "Motion Graphics Designer", 62000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-365)),
                        CreateEmployee(company.Id, "Cynthia", "Gray", "cynthia.gray@creativedesign.com", "+1-555-0608", "CD008", "Content", "Content Strategist", 55000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Edward", "Ramirez", "edward.ramirez@creativedesign.com", "+1-555-0609", "CD009", "Photography", "Photographer", 48000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-90)),
                        CreateEmployee(company.Id, "Helen", "James", "helen.james@creativedesign.com", "+1-555-0610", "CD010", "Administration", "Office Manager", 45000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250))
                    };

                case "Logistics Express Inc":
                    return new[]
                    {
                        CreateEmployee(company.Id, "Joseph", "Watson", "joseph.watson@logisticsexpress.com", "+1-555-0701", "LE001", "Operations", "Operations Director", 105000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-600)),
                        CreateEmployee(company.Id, "Betty", "Brooks", "betty.brooks@logisticsexpress.com", "+1-555-0702", "LE002", "Dispatch", "Dispatch Supervisor", 58000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Gary", "Kelly", "gary.kelly@logisticsexpress.com", "+1-555-0703", "LE003", "Transportation", "Fleet Manager", 72000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-400)),
                        CreateEmployee(company.Id, "Dorothy", "Sanders", "dorothy.sanders@logisticsexpress.com", "+1-555-0704", "LE004", "Customer Service", "Customer Service Representative", 38000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150)),
                        CreateEmployee(company.Id, "George", "Price", "george.price@logisticsexpress.com", "+1-555-0705", "LE005", "Warehouse", "Warehouse Supervisor", 52000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Maria", "Bennett", "maria.bennett@logisticsexpress.com", "+1-555-0706", "LE006", "Safety", "Safety Manager", 68000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Larry", "Wood", "larry.wood@logisticsexpress.com", "+1-555-0707", "LE007", "Maintenance", "Maintenance Coordinator", 48000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Margaret", "Barnes", "margaret.barnes@logisticsexpress.com", "+1-555-0708", "LE008", "Finance", "Billing Specialist", 42000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-365)),
                        CreateEmployee(company.Id, "Eugene", "Ross", "eugene.ross@logisticsexpress.com", "+1-555-0709", "LE009", "IT", "IT Technician", 55000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Deborah", "Henderson", "deborah.henderson@logisticsexpress.com", "+1-555-0710", "LE010", "HR", "HR Coordinator", 50000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300))
                    };

                case "Restaurant Chain Co":
                    return new[]
                    {
                        CreateEmployee(company.Id, "Arthur", "Coleman", "arthur.coleman@restaurantchain.com", "+1-555-0801", "RC001", "Management", "Regional Manager", 88000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-500)),
                        CreateEmployee(company.Id, "Janet", "Jenkins", "janet.jenkins@restaurantchain.com", "+1-555-0802", "RC002", "Operations", "Store Manager", 52000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Ralph", "Perry", "ralph.perry@restaurantchain.com", "+1-555-0803", "RC003", "Kitchen", "Head Chef", 48000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Gloria", "Powell", "gloria.powell@restaurantchain.com", "+1-555-0804", "RC004", "Service", "Front of House Manager", 42000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Harold", "Long", "harold.long@restaurantchain.com", "+1-555-0805", "RC005", "Kitchen", "Line Cook", 32000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Marie", "Patterson", "marie.patterson@restaurantchain.com", "+1-555-0806", "RC006", "Service", "Server", 28000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150)),
                        CreateEmployee(company.Id, "Jerry", "Hughes", "jerry.hughes@restaurantchain.com", "+1-555-0807", "RC007", "Maintenance", "Maintenance Worker", 35000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-365)),
                        CreateEmployee(company.Id, "Kathryn", "Flores", "kathryn.flores@restaurantchain.com", "+1-555-0808", "RC008", "Administration", "Assistant Manager", 38000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Willie", "Washington", "willie.washington@restaurantchain.com", "+1-555-0809", "RC009", "Kitchen", "Kitchen Assistant", 30000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-90)),
                        CreateEmployee(company.Id, "Diane", "Butler", "diane.butler@restaurantchain.com", "+1-555-0810", "RC010", "Service", "Host/Hostess", 28000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-100))
                    };

                case "Educational Resources Ltd":
                    return new[]
                    {
                        CreateEmployee(company.Id, "Walter", "Simmons", "walter.simmons@educationalresources.com", "+1-555-0901", "ER001", "Leadership", "Executive Director", 115000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-800)),
                        CreateEmployee(company.Id, "Joyce", "Foster", "joyce.foster@educationalresources.com", "+1-555-0902", "ER002", "Curriculum", "Curriculum Developer", 68000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-400)),
                        CreateEmployee(company.Id, "Wayne", "Gonzales", "wayne.gonzales@educationalresources.com", "+1-555-0903", "ER003", "Technology", "Educational Technology Specialist", 62000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Frances", "Bryant", "frances.bryant@educationalresources.com", "+1-555-0904", "ER004", "Content", "Content Creator", 55000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Roy", "Alexander", "roy.alexander@educationalresources.com", "+1-555-0905", "ER005", "Sales", "Sales Representative", 48000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Alice", "Russell", "alice.russell@educationalresources.com", "+1-555-0906", "ER006", "Quality", "Quality Assurance Specialist", 52000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Bobby", "Griffin", "bobby.griffin@educationalresources.com", "+1-555-0907", "ER007", "Marketing", "Marketing Specialist", 45000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150)),
                        CreateEmployee(company.Id, "Catherine", "Diaz", "catherine.diaz@educationalresources.com", "+1-555-0908", "ER008", "Customer Support", "Customer Success Manager", 58000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-365)),
                        CreateEmployee(company.Id, "Dennis", "Hayes", "dennis.hayes@educationalresources.com", "+1-555-0909", "ER009", "Research", "Educational Researcher", 65000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Ann", "Myers", "ann.myers@educationalresources.com", "+1-555-0910", "ER010", "Administration", "Administrative Coordinator", 42000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300))
                    };

                case "Construction Masters":
                    return new[]
                    {
                        CreateEmployee(company.Id, "Carl", "Ford", "carl.ford@constructionmasters.com", "+1-555-1001", "CM001", "Management", "Project Manager", 95000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-600)),
                        CreateEmployee(company.Id, "Virginia", "Hamilton", "virginia.hamilton@constructionmasters.com", "+1-555-1002", "CM002", "Engineering", "Site Engineer", 78000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-400)),
                        CreateEmployee(company.Id, "Louis", "Graham", "louis.graham@constructionmasters.com", "+1-555-1003", "CM003", "Construction", "Construction Supervisor", 68000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300)),
                        CreateEmployee(company.Id, "Judith", "Sullivan", "judith.sullivan@constructionmasters.com", "+1-555-1004", "CM004", "Safety", "Safety Coordinator", 62000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-250)),
                        CreateEmployee(company.Id, "Terry", "Wallace", "terry.wallace@constructionmasters.com", "+1-555-1005", "CM005", "Equipment", "Equipment Operator", 52000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-200)),
                        CreateEmployee(company.Id, "Jean", "Woods", "jean.woods@constructionmasters.com", "+1-555-1006", "CM006", "Administration", "Office Administrator", 45000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-180)),
                        CreateEmployee(company.Id, "Philip", "Cole", "philip.cole@constructionmasters.com", "+1-555-1007", "CM007", "Electrical", "Electrician", 58000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-365)),
                        CreateEmployee(company.Id, "Julie", "West", "julie.west@constructionmasters.com", "+1-555-1008", "CM008", "Plumbing", "Plumber", 55000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-150)),
                        CreateEmployee(company.Id, "Sean", "Jordan", "sean.jordan@constructionmasters.com", "+1-555-1009", "CM009", "Carpentry", "Carpenter", 50000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-120)),
                        CreateEmployee(company.Id, "Shirley", "Owens", "shirley.owens@constructionmasters.com", "+1-555-1010", "CM010", "Quality", "Quality Inspector", 48000, EmployeeStatus.Active, DateTime.UtcNow.AddDays(-300))
                    };

                default:
                    return new Employee[0];
            }
        }

        private Employee CreateEmployee(
            Guid companyId,
            string firstName,
            string lastName,
            string email,
            string phone,
            string employeeId,
            string department,
            string position,
            decimal salary,
            EmployeeStatus status,
            DateTime hireDate)
        {
            return new Employee(
                _guidGenerator.Create(),
                firstName,
                lastName,
                email,
                employeeId,
                department,
                position,
                salary,
                hireDate,
                companyId,
                phone,
                status,
                null
            );
        }
    }
}