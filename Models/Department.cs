namespace WUIAM.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int HeadOfDepartmentId { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();

        public Department() { }
        public Department( string name, string description, int headOfDepartmentId)
        {
          
            Name = name;
            Description = description;
            HeadOfDepartmentId = headOfDepartmentId;
        }
    }
}