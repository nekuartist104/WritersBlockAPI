using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;

namespace WritersBlockAPI.Repositories
{
    public interface ILocationRepository
    {
        void Create(CreateLocationRequest createLocationRequest);
        List<Location> All();
        List<Location> All(int worldId);
        Location Find(int locationId);
        void Destroy(int locationId);
    }
}
