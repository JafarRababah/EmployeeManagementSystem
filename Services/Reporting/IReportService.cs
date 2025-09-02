using System.Threading.Tasks;
using System.Collections.Generic;
using EmployeesManagment.ViewModels.Reports;

namespace EmployeesManagment.Services.Reporting
{
    public interface IReportService
    {
        Task<byte[]> EmployeesToExcelAsync(IEnumerable<EmployeeReportRow> rows);
        Task<byte[]> EmployeesToPdfAsync(IEnumerable<EmployeeReportRow> rows);
        Task<byte[]> LeavesToExcelAsync(IEnumerable<LeaveReportRow> rows);
        Task<byte[]> LeavesToPdfAsync(IEnumerable<LeaveReportRow> rows);
        string EmployeesExcelContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        string PdfContentType => "application/pdf";
    }
}
