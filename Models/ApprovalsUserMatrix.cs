﻿namespace EmployeesManagment.Models
{
    public class ApprovalsUserMatrix:UserActivity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int DocumentTypeId { get; set; }
        public SystemCodeDetail DocumentType { get; set; }
        public int workFlowUserGroupId { get; set; }
        public WorkFlowUserGroup WorkFlowUserGroup { get; set; }
        public bool Active { get; set; }
    }
}
