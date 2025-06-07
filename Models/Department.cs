namespace WUIAM.Models
{
    public class Department
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid HeadOfDepartmentId { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();

        public Department() { }
        public Department( string name, string description, Guid headOfDepartmentId)
        {
          
            Name = name;
            Description = description;
            HeadOfDepartmentId = headOfDepartmentId;
        }
    }
}