using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;

namespace WritersBlockAPI.Repositories
{
    public interface IAreaRepository
    {
        void Create(CreateAreaRequest createAreaRequest);
        List<Area> All();
        List<Area> All(int locationId);
        Area Find(int areaId);
        void Destroy(int areaId);
    }
}
