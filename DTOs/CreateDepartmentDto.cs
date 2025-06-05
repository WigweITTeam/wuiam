namespace WUIAM.DTOs
{
    public class CreateDepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int HeadOfDepartmentId { get; set; }
    }
}
