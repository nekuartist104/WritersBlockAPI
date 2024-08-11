namespace WritersBlockAPI.Models
{
    public class Area
    {
        public int AreaId { get; set; }
        public int LocationId { get; set; }
        public int AreaTypeId { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
