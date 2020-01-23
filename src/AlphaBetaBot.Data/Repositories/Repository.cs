using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly DbSet<TEntity> _entities;
        protected readonly AbfDbContext _context;

        protected readonly string _name;

        protected Repository(DbSet<TEntity> entities, AbfDbContext context, string name)
        {
            _entities = entities;
            _context = context;

            _name = name;
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync()
        {
            try
            {
                var entities = await _entities.ToListAsync();

                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.GetAll,
                    Path = $"::/{_name}Repository/{ActionType.GetAll}"
                });

                return entities.AsReadOnly();
            }
            catch (Exception ex)
            {
                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.GetAll,
                    Path = $"::/{_name}Repository/{ActionType.GetAll}",
                    IsErrored = true,
                    Exception = ex
                });

                return new List<TEntity>();
            }
        }

        public virtual async Task<TEntity> GetAsync(ulong id)
        {
            try
            {
                var entity = await _entities.FindAsync(id);

                if (entity is null)
                {
                    return null;
                }

                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.Get,
                    Path = $"::/{_name}Repository/{ActionType.Get}/{id}"
                });

                return entity;
            }
            catch (Exception ex)
            {
                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.Get,
                    Path = $"::/{_name}Repository/{ActionType.Get}/{id}",
                    IsErrored = true,
                    Exception = ex
                });

                return null;
            }
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            try
            {
                var data = (await _entities.AddAsync(entity)).Entity;

                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.Add,
                    Path = $"::/{_name}Repository/{ActionType.Add}/{entity.Id}"
                });

                return data;
            }
            catch (Exception ex)
            {
                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.Add,
                    Path = $"::/{_name}Repository/{ActionType.Add}/{entity.Id}",
                    IsErrored = true,
                    Exception = ex
                });

                return null;
            }
        }

        public virtual Task DeleteAsync(TEntity entity)
        {
            try
            {
                _entities.Remove(entity);

                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.Delete,
                    Path = $"::/{_name}Repository/{ActionType.Delete}/{entity.Id}"
                });
            }
            catch (Exception ex)
            {
                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.Delete,
                    Path = $"::/{_name}Repository/{ActionType.Delete}/{entity.Id}",
                    IsErrored = true,
                    Exception = ex
                });
            }

            return Task.CompletedTask;
        }

        public virtual async Task DeleteAllAsync()
        {
            try
            {
                _entities.RemoveRange(await GetAllAsync());

                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.DeleteAll,
                    Path = $"::/{_name}Repository/{ActionType.DeleteAll}"
                });
            }
            catch (Exception ex)
            {
                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.DeleteAll,
                    Path = $"::/{_name}Repository/{ActionType.DeleteAll}",
                    IsErrored = true,
                    Exception = ex
                });
            }
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            try
            {
                _entities.Update(entity);

                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.Update,
                    Path = $"::/{_name}Repository/{ActionType.Update}/{entity.Id}"
                });
            }
            catch (Exception ex)
            {
                _context.InvokeEvent(new DatabaseActionEventArgs
                {
                    ActionType = ActionType.Update,
                    Path = $"::/{_name}Repository/{ActionType.Update}/{entity.Id}",
                    IsErrored = true,
                    Exception = ex
                });
            }

            return Task.CompletedTask;
        }
    }
}
