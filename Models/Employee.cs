using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace EmployeesManagment.Models
{
    public class Employee:UserActivity
    {
        public int Id { get; set; }
        public string EmpNo { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}";
        public int PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public int? CountryId { get; set; }
        public Country Country { get; set; }
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="0:yyyy/MM/dd")]
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }
        public int? DesignationId { get; set; }
        public Designation Designation { get; set; }
        public int? GenderId { get; set; }
        public SystemCodeDetail Gender { get; set; }
        public string? Photo { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public int? StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "0:yyyy/MM/dd")]
        public DateTime? InactiveDate { get; set; }
        public int? CauseOfInactivityId { get; set; }
        public SystemCodeDetail CauseOfInactivity { get; set; }
        public DateTime? TerminationDate { get; set; }
        public int? ReasonForTerminationId { get; set; }
        public SystemCodeDetail ReasonForTermination { get; set; }
        public int? BankId { get; set; }
        public Bank Bank { get; set; }
        public string? BankAccountNo { get; set; }
        public string? IBAN { get; set; }
        public string? SWIFTCode { get; set; }
        public string? NSSFNO { get; set; }
        public string? NHIF { get; set; }
        public string? CompanyEmail { get; set; }
        public string? KRAPIN { get; set; }
        public string? PassportNo { get; set; }
        public int? EmploymentTermsId { get; set; }
        public SystemCodeDetail EmploymentTerms { get; set; }
        public Decimal? AllocatedLeaveDays { get; set; }
        public Decimal? LeaveOutStandingBalance { get; set; }
        public bool? PaysTax {  get; set; }
        public int? DisabilityId { get; set; }
        public SystemCodeDetail Disability { get; set; }
        public string? DisabilityCertificate { get; set; }

    }
}
