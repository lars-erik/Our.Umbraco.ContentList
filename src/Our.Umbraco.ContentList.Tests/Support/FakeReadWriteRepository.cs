using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Cms.Core.Persistence.Querying;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public class FakeReadWriteRepository<TKey, T> : IReadWriteQueryRepository<TKey, T>
    {
        protected readonly Func<T, TKey> IdAccessor;
        protected Dictionary<TKey, T> Items = new Dictionary<TKey, T>();

        public FakeReadWriteRepository(Func<T, TKey> idAccessor)
        {
            this.IdAccessor = idAccessor;
        }

        public T Get(TKey id)
        {
            return Items[id];
        }

        public IEnumerable<T> GetMany(params TKey[] ids)
        {
            return Items.Where(x => ids.Contains(x.Key)).Select(x => x.Value);
        }

        public bool Exists(TKey id)
        {
            return Items.ContainsKey(id);
        }

        public void Save(T entity)
        {
            var id = IdAccessor(entity);
            if (Items.ContainsKey(id))
            {
                Items[id] = entity;
            }
            else
            {
                Items.Add(id, entity);
            }
        }

        public void Delete(T entity)
        {
            var id = IdAccessor(entity);
            if (Items.ContainsKey(id))
            {
                Items.Remove(id);
            }
        }

        public IEnumerable<T> Get(IQuery<T> query)
        {
            return Items.Values;
        }

        public int Count(IQuery<T> query)
        {
            return Items.Values.Count;
        }
    }
}