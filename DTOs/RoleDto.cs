namespace WUIAM.DTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class RoleCreateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class RoleUpdateDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
