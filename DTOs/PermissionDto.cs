using System;

namespace WUIAM.DTOs
{
    public class PermissionDto
    {
       
        public string Name { get; set; }
        public string? Description { get; set; }
    }
    public class GrantRolePermissionDto
    {
        public Guid RoleId { get; set; }
        public string Permission { get; set; }
    }
    public class GrantPermissionDto
    {
        public Guid UserId { get; set; }
        public string Permission { get; set; }
    }
}