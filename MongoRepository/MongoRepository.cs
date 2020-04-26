using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using MongoDB.Driver;

namespace R5.MongoRepository
{
	public abstract class MongoRepository<TAggregate, TDocument, TId> : IRepository<TAggregate, TId>, IAggregateOperationStore
		where TAggregate : IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		protected readonly IMongoSessionDbContext _dbContext;
		protected readonly IAggregateMapper<TAggregate, TDocument, TId> _mapper;
		protected readonly IAggregateIdentityMap<TAggregate, TId> _identityMap;

		protected MongoRepository(
			IMongoSessionDbContext dbContext,
			IAggregateMapper<TAggregate, TDocument, TId> mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		public virtual async Task<Option<TAggregate>> FindOrNone(TId id)
		{
			if (_identityMap.TryGet(id, out TAggregate aggregate))
			{
				return Some(aggregate);
			}

			var collection = _dbContext.GetCollection<TDocument>();
			
			TDocument document = await collection
				.Find(Builders<TDocument>.Filter.Eq(d => d.Id, id))
				.SingleOrDefaultAsync();

			if (document == null)
			{
				return None;
			}

			aggregate = _mapper.ToAggregate(document);
			_identityMap.Set(id, aggregate);

			return aggregate;
		}

		public virtual Task Add(TAggregate aggregate)
		{
			throw new NotImplementedException();
		}

		public virtual Task Delete(TAggregate aggregate)
		{
			throw new NotImplementedException();
		}

		public List<ICommitAggregateSaveOperation> GetCommitOperations()
		{
			throw new NotImplementedException();
		}
	}
}
