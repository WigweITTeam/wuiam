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
        public required string UserEmail { get; set; }

        [Unicode(false)]
        public required string Password { get; set; }


        public DateTime DateCreated { get; set; }
         
        public Guid? DepartmentId { get; set; }
        public Guid UserTypeId { get;  set; }
        public required string LastName { get;  set; }
        public required string FirstName { get;  set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Guid UserTypeId { get; set; }
        public Guid DepartmentId { get; set; }
    }
}


