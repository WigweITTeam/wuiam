using System.Security.Cryptography;
using System.Text;
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
            // seeding logic for admin user
            if (!_context.Roles.Any())
            {
                _context.Roles.Add(new Role
                {
                    Name = "Admin",
                    Description = "Administrator role with full access",

                });
                  _context.SaveChanges();
            }
            if (!_context.Departments.Any())
            {
                _context.Departments.Add(new Department
                {
                    Name = "General Administration",
                    Description = "Main administrative department",
                    HeadOfDepartmentId = 1, // Assuming admin user is the head of this department
                    // CreatedById = 1 // Assuming admin user created this department
                });
                  _context.SaveChanges();
            }
            if (!_context.UserTypes.Any())
            {
                _context.UserTypes.AddRange(new List<UserType>
                {
                    new UserType { Name = "Student", Description = "Regular student user" },
                    new UserType { Name = "Staff", Description = "Staff user" },
                    new UserType { Name = "Lecturer", Description = "Lecturer user" }
                });
                  _context.SaveChanges();
            }
            if (!_context.Users.Any())
            {
                if (_context.Users.FirstOrDefault(u => u.UserEmail == "admin@example.com") == null)
                {
                    _context.Users.Add(new User
                    {
                        UserName = "admin",
                        UserEmail = "admin@example.com",
                        Password = PasswordUtilService.HashPassword("admin103#,"),
                        DateCreated = DateTime.Now,
                        IsDefault = true,
                        FullName = "Administrator",
                        UserTypeId = _context.UserTypes.FirstOrDefault()?.Id ?? 1, // Assuming default user type
                        CreatedById = 1, // Assuming admin user is created by itself
                        SingleSignOnEnabled = false,
                        SessionId = Guid.NewGuid().ToString(),
                        SessionTime = DateTime.Now,
                        DeptId = _context.Departments.FirstOrDefault()?.Id ??1// Assuming no department for admin
                    });
                    _context.SaveChanges();
                }
                if (!_context.UserRoles.Any())
                {
                    var existingUser = _context.Users.First();
                    var existingRole = _context.Roles.First();

                    var userRole = new UserRole
                    {
                        UserId = existingUser.UserId,
                        RoleId = existingRole.Id
                    };

                    _context.UserRoles.Add(userRole);
                    _context.SaveChanges();
                }


            }
        }
    }
}