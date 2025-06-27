using System.ComponentModel.DataAnnotations;
namespace WUIAM.Enums
{
    public enum Roles
    {
        [Display(Name = "Super Administrator")]
        SuperAdmin,

        [Display(Name = "IT Administrator")]
        ITAdmin,

        [Display(Name = "HR Manager")]
        HRManager,

        [Display(Name = "HR Officer")]
        HROfficer,

        [Display(Name = "Recruitment Officer")]
        Recruiter,

        [Display(Name = "Payroll Officer")]
        PayrollOfficer,

        [Display(Name = "Head of Department")]
        DepartmentHead,

        [Display(Name = "Line Manager")]
        LineManager,

        [Display(Name = "Employee")]
        Employee,

        [Display(Name = "Intern")]
        Intern,

        [Display(Name = "Contract Staff")]
        Contractor
    }
}