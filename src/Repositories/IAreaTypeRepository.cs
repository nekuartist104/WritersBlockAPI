using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;

namespace WritersBlockAPI.Repositories
{
    public interface IAreaTypeRepository
    {
        void Create(CreateAreaTypeRequest createAreaTypeRequest);
        List<AreaType> All();
        AreaType Find(int areaTypeId);
        void Destroy(int areaTypeId);
    }
}
