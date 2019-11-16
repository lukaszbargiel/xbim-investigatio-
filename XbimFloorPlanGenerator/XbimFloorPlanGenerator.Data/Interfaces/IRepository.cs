using System.Linq;
using System.Threading.Tasks;

namespace XbimFloorPlanGenerator.Data.Interfaces
{
    public interface IRepository<TEntity>
 where TEntity : class, IEntity
    {
        IQueryable<TEntity> GetAll();

        TEntity GetById(int id);

        int Create(TEntity entity);

        void Update(int id, TEntity entity);

        void Delete(int id);
    }
}

