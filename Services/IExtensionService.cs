using EmployeesManagment.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagment.Services
{
    
    public interface IExtensionService
    {
        Task<string> GenerateEmployeeNumber();
        Task<string> GenerateBankNumber();
        Task<string> GenerateCountryNumber();
        Task<string> GenerateCityNumber();
        Task<string> GenerateDepartmentNumber();
        Task<string> GenerateDesignationNumber();
        Task<string> GenerateAssetNumber();
        Task<string> GenerateLeaveTypeNumber();
    }
    public class ExtensionService : IExtensionService
    {
        private readonly ApplicationDbContext _context;
        public ExtensionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<string> GenerateEmployeeNumber()
        {
            string employeeNumber;
            bool exists;
            Random _randomizer=new Random();
            do
            {
                int randomnumber = _randomizer.Next(1000, 9999);
                employeeNumber=$"EPN{randomnumber}";
                exists=await _context.Employees.AnyAsync(e=>e.EmpNo==employeeNumber);
            }while(exists);
            return employeeNumber;
        }
        public async Task<string> GenerateBankNumber()
        {
            string bankNumber;
            bool exists;
            Random _randomizer = new Random();
            do
            {
                int randomnumber = _randomizer.Next(100, 9999);
                bankNumber=$"BNK{randomnumber}";
                exists=await _context.Banks.AnyAsync(e => e.Code==bankNumber);
            } while (exists);
            return bankNumber;
        }
        public async Task<string> GenerateCountryNumber()
        {
            string countryNumber;
            bool exists;
            Random _randomizer = new Random();
            do
            {
                int randomnumber = _randomizer.Next(100, 9999);
                countryNumber = $"CTR{randomnumber}";
                exists = await _context.Countries.AnyAsync(e => e.Code == countryNumber);
            } while (exists);
            return countryNumber;
        }
        public async Task<string> GenerateCityNumber()
        {
            string cityNumber;
            bool exists;
            Random _randomizer = new Random();
            do
            {
                int randomnumber = _randomizer.Next(100, 9999);
                cityNumber = $"ST{randomnumber}";
                exists = await _context.Cities.AnyAsync(e => e.Code == cityNumber);
            } while (exists);
            return cityNumber;
        }
        public async Task<string> GenerateDepartmentNumber()
        {
            string departmentNumber;
            bool exists;
            Random _randomizer = new Random();
            do
            {
                int randomnumber = _randomizer.Next(100, 9999);
                departmentNumber = $"DPT{randomnumber}";
                exists = await _context.Departments.AnyAsync(e => e.Code == departmentNumber);
            } while (exists);
            return departmentNumber;
        }
        public async Task<string> GenerateDesignationNumber()
        {
            string designationNumber;
            bool exists;
            Random _randomizer = new Random();
            do
            {
                int randomnumber = _randomizer.Next(100, 9999);
                designationNumber = $"DS{randomnumber}";
                exists = await _context.Designations.AnyAsync(e => e.Code == designationNumber);
            } while (exists);
            return designationNumber;
        }
        public async Task<string> GenerateAssetNumber()
        {
            string assetNumber;
            bool exists;
            Random _randomizer = new Random();
            do
            {
                int randomnumber = _randomizer.Next(100, 9999);
                assetNumber = $"DS{randomnumber}";
                exists = await _context.FixedAssets.AnyAsync(e => e.AssetNo == assetNumber);
            } while (exists);
            return assetNumber;
        }
        public async Task<string> GenerateLeaveTypeNumber()
        {
            string leaveTypeNumber;
            bool exists;
            Random _randomizer = new Random();
            do
            {
                int randomnumber = _randomizer.Next(10, 99);
                leaveTypeNumber = $"LT{randomnumber}";
                exists = await _context.LeaveTypes.AnyAsync(e => e.Code == leaveTypeNumber);
            } while (exists);
            return leaveTypeNumber;
        }
    }
}
