using EmployeesManagment.Models;

namespace EmployeesManagment.ViewModels
{
    public class ProfileViewModel
    {
        public ICollection<SystemProfile> Profiles { get; set; }
        public ICollection<int> RolesRightsIds { get; set; }
        public int[] Ids { get; set; }
        public string RoleId {  get; set; }
        public int TaskId { get; set; }
    }
}
