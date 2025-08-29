namespace EmployeesManagment.Models
{
    public class ApprovalEntry
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public int DocumentTypeId { get; set; }
        public SystemCodeDetail DocumentType { get; set; }
        public int SequenceNo { get; set; }
        public string ApproverId { get; set; }
        public ApplicationUser Approver { get; set; }
        public int StatusId { get; set; }
        public SystemCodeDetail Status { get; set; }
        public DateTime DateSentForApproval { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string LastModifiedId { get; set; }
        public ApplicationUser LastModifiedBy { get; set; }
        public string Comments { get; set; }
        public string ControllerName { get; set; }
    }
}
