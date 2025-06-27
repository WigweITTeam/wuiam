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
                    _context.Roles.Add(new Role
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
                _context.Departments.Add(new Department
                {
                    Name = "ICT",
                    Description = "Main ICT department",
                    HeadOfDepartmentId = Guid.NewGuid(),
                });
                _context.Departments.Add(new Department
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
                        new UserType { Name = "Student", Description = "Regular student user" },
                        new UserType { Name = "Staff", Description = "Staff user" },
                        new UserType { Name = "Lecturer", Description = "Lecturer user" },
                        new UserType { Name = "Contract", Description = "Contract staff user" }
                    });
                _context.SaveChanges();
            }

            // Seed Permissions
            if (!_context.Permissions.Any())
            {
                var permissionNames = Enum.GetNames(typeof(Permissions));
                foreach (var permissionName in permissionNames)
                {
                    _context.Permissions.Add(new Permission
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
                _context.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    AssignedAt = DateTime.Now
                });
                _context.SaveChanges();

                // Assign AdminAccess Permission to Admin Role
                var adminAccessPermission = _context.Permissions.FirstOrDefault(p => p.Name == "AdminAccess");
                if (adminAccessPermission != null && !_context.RolePermissions.Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == adminAccessPermission.Id))
                {
                    _context.RolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = adminAccessPermission.Id,
                        GrantedAt = DateTime.Now
                    });
                    _context.SaveChanges();
                }
            }
        }
    }
}