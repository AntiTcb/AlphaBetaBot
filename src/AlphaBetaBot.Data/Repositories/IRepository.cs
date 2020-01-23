using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlphaBetaBot.Data
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();

        Task<TEntity> GetAsync(ulong id);

        Task<TEntity> AddAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task DeleteAllAsync();

        Task UpdateAsync(TEntity entity);
    }
    public interface IGetOrAddRepository<T> : IRepository<T> where T : Entity
    {
        Task<T> GetOrAddAsync(ulong id);
    }
}
