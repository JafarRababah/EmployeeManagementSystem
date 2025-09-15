using EmployeesManagment.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeesManagment.Services
{
    
    public interface IExtensionService
    {
        Task<string> GenerateEmployeeNumber();
        Task<string> GenerateBankNumber();
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

    }
}
