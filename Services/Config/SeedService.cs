using System.Security.Cryptography;
using System.Text;
using WUIAM.Enums;
using WUIAM.Enums;
using WUIAM.Models;

namespace WUIAM.Services.Config.SeedService
{
    public class SeedService
    {
        private readonly WUIAMDbContext _context;

        public SeedService(WUIAMDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            // Seed Roles  
            if (!_context.Roles.Any())
            {
                var roleNames = Enum.GetNames(typeof(Roles));
                foreach (var role in roleNames)
                {
                    _context.Roles.Add(new()
                    {
                        Name = role,
                        Description = $"The {role} role access"
                    });
                }
                _context.SaveChanges();
            }

            // Seed Departments  
            if (!_context.Departments.Any())
            {
                _context.Departments.Add(new()
                {
                    Name = "ICT",
                    Description = "Main ICT department",
                    HeadOfDepartmentId = Guid.NewGuid(),
                });
                _context.Departments.Add(new()
                {
                    Name = "HR",
                    Description = "The HR department",
                    HeadOfDepartmentId = Guid.NewGuid(),
                });
                _context.SaveChanges();
            }

            // Seed UserTypes  
            if (!_context.UserTypes.Any())
            {
                _context.UserTypes.AddRange(new List<UserType>
                   {
                       new() { Name = "Staff", Description = "Staff user" },
                       new() { Name = "Student", Description = "Regular student user" },
                       new() { Name = "Lecturer", Description = "Lecturer user" },
                       new() { Name = "Contract", Description = "Contract staff user" }
                   });
                _context.SaveChanges();
            }

            // Seed Permissions  
            if (!_context.Permissions.Any())
            {
                var permissionNames = Enum.GetNames(typeof(Permissions));
                foreach (var permissionName in permissionNames)
                {
                    _context.Permissions.Add(new()
                    {
                        Name = permissionName,
                        Description = $"Permission to {permissionName.ToLower()}"
                    });
                }
                _context.SaveChanges();
            }

            // Seed Admin User  
            if (!_context.Users.Any())
            {
                var adminUserTypeId = _context.UserTypes.FirstOrDefault()?.Id ?? Guid.NewGuid();
                var adminDeptId = _context.Departments.FirstOrDefault()?.Id ?? Guid.NewGuid();

                var adminUser = new User
                {
                    UserName = "SuperAdmin",
                    UserEmail = "standevcode@gmail.com",
                    Password = PasswordUtilService.HashPassword("admin103#,"),
                    DateCreated = DateTime.Now,
                    IsDefault = true,
                    FullName = "Administrator",
                    UserTypeId = adminUserTypeId,
                    CreatedById = Guid.NewGuid(),
                    SingleSignOnEnabled = false,
                    SessionId = Guid.NewGuid().ToString(),
                    SessionTime = DateTime.Now,
                    TwoFactorEnabled = true,
                    DeptId = adminDeptId
                };
                _context.Users.Add(adminUser);
                _context.SaveChanges();

                // Assign Admin Role to Admin User  
                var adminRole = _context.Roles.FirstOrDefault(r => r.Name == "Admin") ?? _context.Roles.First();
                _context.UserRoles.Add(new()
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.Now
                });
                _context.SaveChanges();

                // Assign AdminAccess Permission to Admin Role  
                var adminAccessPermission = _context.Permissions.FirstOrDefault(p => p.Name == Permissions.AdminAccess.ToString());
                if (adminAccessPermission != null && !_context.RolePermissions.Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == adminAccessPermission.Id))
                {
                    _context.RolePermissions.Add(new()
                    {
                        RoleId = adminRole.Id,
                        PermissionId = adminAccessPermission.Id,
                        GrantedAt = DateTime.Now
                    });
                    _context.UserPermissions.Add(new UserPermission { UserId = adminUser.Id, PermissionId = adminAccessPermission!.Id });

                    _context.SaveChanges();
                }
                 
            }
        }
    }
}