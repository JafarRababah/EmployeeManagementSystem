using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using EmployeesManagment.ViewModels.Reports;

namespace EmployeesManagment.Services.Reporting
{
    public class ReportService : IReportService
    {
        // ---------------- Excel Reports ----------------
        public async Task<byte[]> EmployeesToExcelAsync(IEnumerable<EmployeeReportRow> rows)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Employees");

            var headers = new[] { "Full Name", "Department", "Title", "Email", "Phone", "Hired On" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
                ws.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int r = 2;
            foreach (var x in rows)
            {
                ws.Cells[r, 1].Value = x.FullName;
                ws.Cells[r, 2].Value = x.Department;
                ws.Cells[r, 3].Value = x.JobTitle;
                ws.Cells[r, 4].Value = x.Email;
                ws.Cells[r, 5].Value = x.Phone;
                ws.Cells[r, 6].Value = x.HiredOn?.ToString("yyyy-MM-dd");
                r++;
            }

            ws.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }

        public async Task<byte[]> LeavesToExcelAsync(IEnumerable<LeaveReportRow> rows)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Leaves");

            var headers = new[] { "Employee", "Type", "Start", "End", "Total Days", "Status", "Notes" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
                ws.Cells[1, i + 1].Style.Font.Bold = true;
                ws.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            int r = 2;
            foreach (var x in rows)
            {
                ws.Cells[r, 1].Value = x.EmployeeName;
                ws.Cells[r, 2].Value = x.LeaveType;
                ws.Cells[r, 3].Value = x.StartDate.ToString("yyyy-MM-dd");
                ws.Cells[r, 4].Value = x.EndDate.ToString("yyyy-MM-dd");
                ws.Cells[r, 5].Value = x.TotalDays;
                ws.Cells[r, 6].Value = x.Status;
                ws.Cells[r, 7].Value = x.Notes;
                r++;
            }

            ws.Cells.AutoFitColumns();
            return await Task.FromResult(package.GetAsByteArray());
        }

        // ---------------- PDF Reports ----------------
        public async Task<byte[]> EmployeesToPdfAsync(IEnumerable<EmployeeReportRow> rows)
        {
            var list = rows.ToList();
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(35);
                    page.Header().Text("Employees Report").SemiBold().FontSize(18);
                    page.Footer().AlignRight().Text(text =>
                    {
                        text.Span("Generated ").Light();
                        text.Span(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(130);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.ConstantColumn(90);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(c => CellHeader(c)).Text("Full Name");
                            header.Cell().Element(c => CellHeader(c)).Text("Department");
                            header.Cell().Element(c => CellHeader(c)).Text("Title");
                            header.Cell().Element(c => CellHeader(c)).Text("Email");
                            header.Cell().Element(c => CellHeader(c)).Text("Phone");
                            header.Cell().Element(c => CellHeader(c)).Text("Hired On");
                        });

                        int i = 0;
                        foreach (var x in list)
                        {
                            bool alt = i++ % 2 == 1;
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.FullName ?? "");
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.Department ?? "");
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.JobTitle ?? "");
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.Email ?? "");
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.Phone ?? "");
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.HiredOn?.ToString("yyyy-MM-dd") ?? "");
                        }
                    });
                });
            });

            return await Task.FromResult(doc.GeneratePdf());
        }

        public async Task<byte[]> LeavesToPdfAsync(IEnumerable<LeaveReportRow> rows)
        {
            var list = rows.ToList();
            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(35);
                    page.Header().Text("Leaves Report").SemiBold().FontSize(18);
                    page.Footer().AlignRight().Text(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.ConstantColumn(90);
                            columns.ConstantColumn(90);
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(c => CellHeader(c)).Text("Employee");
                            header.Cell().Element(c => CellHeader(c)).Text("Type");
                            header.Cell().Element(c => CellHeader(c)).Text("Start");
                            header.Cell().Element(c => CellHeader(c)).Text("End");
                            header.Cell().Element(c => CellHeader(c)).Text("Days");
                            header.Cell().Element(c => CellHeader(c)).Text("Status");
                            header.Cell().Element(c => CellHeader(c)).Text("Notes");
                        });

                        int i = 0;
                        foreach (var x in list)
                        {
                            bool alt = i++ % 2 == 1;
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.EmployeeName ?? "");
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.LeaveType ?? "");
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.StartDate.ToString("yyyy-MM-dd"));
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.EndDate.ToString("yyyy-MM-dd"));
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.TotalDays.ToString());
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.Status ?? "");
                            table.Cell().Element(c => CellBody(c, alt)).Text(x.Notes ?? "");
                        }
                    });
                });
            });

            return await Task.FromResult(doc.GeneratePdf());
        }

        // ---------------- Helpers ----------------
        private static IContainer CellHeader(IContainer c) =>
            c.Padding(4)
             .Background(Colors.Grey.Lighten2)
             .Border(1)
             .BorderColor(Colors.Grey.Lighten1)
             .DefaultTextStyle(x => x.SemiBold());

        private static IContainer CellBody(IContainer c, bool alt) =>
            c.Padding(4)
             .Border(1)
             .BorderColor(Colors.Grey.Lighten3)
             .Background(alt ? Colors.Grey.Lighten5 : Colors.White);
    }
}
