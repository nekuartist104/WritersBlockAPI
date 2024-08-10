using WritersBlockAPI.Controllers.Requests;
using WritersBlockAPI.Models;

namespace WritersBlockAPI.Repositories
{
    public interface IWorldRepository
    {
        void Create(CreateWorldRequest createWorldRequest);
        List<World> All();
        World Find(int worldId);
        void Destroy(int worldId);
    }
}
