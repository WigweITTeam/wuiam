using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using WUIAM.Models;
using Newtonsoft.Json;
namespace WUIAM
{
    public class User
    {
        private UserType userType;

        [Key]
        public Guid Id { get; set; }

        [StringLength(90)]
        [Unicode(false)]
        public string? UserName { get; set; }

        [StringLength(90)]
        [Unicode(false)]
        public required string FirstName { get; set; }

        [StringLength(90)]
        [Unicode(false)]
        public required string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [Unicode(false)]
        public required string UserEmail { get; set; }

        [Required]
        [Unicode(false)]
        public required string Password { get; set; }

        public string? ResetPasswordToken { get; set; }

        public bool IsDefault { get; set; }

        public DateTime? DateLastLoggedIn { get; set; }

        [Column("CreatedByID")]
        public Guid CreatedById { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateCreated { get; set; }

        [Column("SingleSignOnEnabled")]
        public bool SingleSignOnEnabled { get; set; }

        [Column("SessionID")]
        [Unicode(false)]
        public string? SessionId { get; set; }

        public DateTime? SessionTime { get; set; }

        public Guid UserTypeId { get; set; }
        [ForeignKey("UserTypeId")]
        public UserType UserType { get => userType; set => userType = value; }

        [Column("DeptID")]
        public Guid? DeptId { get; set; }
        public Department? Department { get; set; }

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public bool TwoFactorEnabled { get; set; } = true;

        [JsonIgnore]
        public List<MFAToken> MFATokens { get; set; } = new List<MFAToken>();
        public Guid EmploymentTypeId { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public LeaveBalance LeaveBalance { get;  set; }
    }

    public class EmploymentType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }       // e.g., "FullTime", "Contract", "Adjunct"
        public string Description { get; set; } // Optional: e.g., "Full-time permanent staff"
        public bool IsActive { get; set; } = true;
    }

}