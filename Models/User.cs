using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WUIAM.Models
{
  [Index(nameof(UserEmail), IsUnique = true)]
  public class User
  {
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(90)]
    [Unicode(false)]
    public string? UserName { get; set; }

    [StringLength(60)]
    [Unicode(false)]
    public string? FullName { get; set; }

    [Required]
    [Unicode(false)]
    public string? UserEmail { get; set; }

    [Unicode(false)]
    public string? Password { get; set; }

    public string? ResetPassordToken { get; set; }

    public bool IsDefault { get; set; }

    public DateTime? DateLastLoggedIn { get; set; }

    [Column("CreatedByID")]
    public int CreatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    [Column("SingleSIgnOnEnabled")]
    public bool SingleSignOnEnabled { get; set; }



    [Column("SessionID")]
    [Unicode(false)]
    public string? SessionId { get; set; }

    public DateTime? SessionTime { get; set; }

    // [Column("RoleID")]
    // public int RoleId { get; set; }
    public int UserTypeId { get; set; }
    [ForeignKey("UserTypeId")]
    UserType UserType { get; set; }

    [Column("DeptID")]
    public int? DeptId { get; set; }
    public Department Department { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public bool TwoFactorEnabled { get; set; } = true;

    [JsonIgnore] 
    public List<MFAToken> MFATokens { get; set; }

  }
}