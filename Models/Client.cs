﻿namespace EmployeesManagment.Models
{
    public class Client:UserActivity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int StatusId { get; set; }
        public SystemCodeDetail Status {  get; set; }
       

    }
}
