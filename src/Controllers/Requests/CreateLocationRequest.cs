namespace WritersBlockAPI.Controllers.Requests
{
    public class CreateLocationRequest
    {
        public int WorldId { get; set; }
        public string Name { get; set; }
        public string Nationality { get; set; }
        public int Population { get; set; }
        public string Climate { get; set; }
        public string Terrain { get; set; }
    }
}
