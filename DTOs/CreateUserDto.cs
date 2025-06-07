using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WUIAM.Models;

namespace WUIAM.DTOs
{
    public class CreateUserDto
    {
        [StringLength(90)]
        [Unicode(false)]
        public string? UserName { get; set; }

        [StringLength(60)]
        [Unicode(false)]
        public string? FullName { get; set; }

        [StringLength(150)]
        [Unicode(false)]
        public string? UserEmail { get; set; }

        [Unicode(false)]
        public string? Password { get; set; }


        public DateTime DateCreated { get; set; }
         
        public Guid? DepartmentId { get; set; }
        public Guid UserTypeId { get;  set; }
    }
}


