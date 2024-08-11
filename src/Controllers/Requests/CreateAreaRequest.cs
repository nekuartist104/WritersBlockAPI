namespace WritersBlockAPI.Controllers.Requests
{
    public class CreateAreaRequest
    {
        public int LocationId { get; set; }
        public int AreaTypeId { get; set; }
        public string Name { get; set; }
        public int Size { get; set; }
    }
}
