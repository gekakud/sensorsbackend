﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using MongoDB.Driver;

namespace Core.Common.Mongo
{
    public class DataRepository<TEntity> : IDataRepository<TEntity> where TEntity : IIdentifiable
    {
        protected IMongoCollection<TEntity> Collection { get; }

        public DataRepository(IMongoDatabase database, string collectionName)
        {
            Collection = database.GetCollection<TEntity>(collectionName);
        }

        public async Task<TEntity> GetAsync(Guid id)
            => await GetAsync(e => e.Id == id);

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
            => await Collection.Find(predicate).SingleOrDefaultAsync();

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
            => await Collection.Find(predicate).ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await Collection.Find(x=>true).ToListAsync();

        public async Task AddAsync(TEntity entity)
            => await Collection.InsertOneAsync(entity);

        public async Task UpdateAsync(TEntity entity)
            => await Collection.ReplaceOneAsync(e => e.Id == entity.Id, entity);

        public async Task DeleteAsync(Guid id)
            => await Collection.DeleteOneAsync(e => e.Id == id);

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
            => await Collection.Find(predicate).AnyAsync();
    }
}
