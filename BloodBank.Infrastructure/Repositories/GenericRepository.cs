using BloodBank.Core.Entities;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly BloodBankDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository ( BloodBankDbContext context )
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T> GetByIdAsync ( int id )
        {
            return await _dbSet.FindAsync( id );
        }

        public async Task<IEnumerable<T>> GetAllAsync ()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> AddAsync ( T entity )
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync( entity );
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync ( T entity )
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Entry( entity ).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync ( T entity )
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync( entity );
        }
    }
}
