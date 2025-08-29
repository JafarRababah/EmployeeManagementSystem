using AutoMapper;
using EmployeesManagment.Models;
using EmployeesManagment.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace EmployeesManagment.Views.Profiles
{
    public class AutomapperProfiles:Profile
    {
      public AutomapperProfiles() 
        {
            CreateMap<Employee, EmployeeViewModel>().ReverseMap();
        }
    }
}
