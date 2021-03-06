﻿using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
//using LanguageExt;
using MongoDB.Driver;
using System.Threading;
using Ardalis.GuardClauses;
using System.Linq;
using MongoDB.Bson;

namespace R5.MongoRepository
{
	public abstract class MongoRepository<TAggregate, TDocument, TId> : IRepository<TAggregate, TId>//, IAggregateOperationStore
		where TAggregate : class, IAggregateRoot<TId>
		where TDocument : IAggregateDocument<TId>
	{
		protected readonly IMongoSessionContext _dbContext;
		protected readonly IAggregateMapper<TAggregate, TDocument, TId> _mapper;
		protected readonly AggregateIdentityMap<TAggregate, TId> _identityMap = new AggregateIdentityMap<TAggregate, TId>();
		private SemaphoreSlim _identityMapLock { get; } = new SemaphoreSlim(1);

		protected MongoRepository(
			IMongoSessionContext dbContext,
			IAggregateMapper<TAggregate, TDocument, TId> mapper)
		{
			_dbContext = dbContext;
			_mapper = mapper;
		}

		public virtual async Task<TAggregate> FindOrDefault(TId id)
		{
			if (_identityMap.TryGet(id, out TAggregate aggregate))
			{
				return aggregate;
			}

			await _identityMapLock.WaitAsync();
			try
			{
				if (!_identityMap.ContainsEntry(id))
				{
					var collection = _dbContext.GetCollection<TDocument>();

					TDocument document = await collection
						.Find(Builders<TDocument>.Filter.Eq(d => d.Id, id))
						.SingleOrDefaultAsync();

					//var document = await collection.FindOneAndUpdateAsync(
					//	Builders<TDocument>.Filter.Eq(d => d.Id, id),
					//	Builders<TDocument>.Update.Set(d => d.SessionLock, Guid.NewGuid().ToString()));

					if (document == null)
					{
						return null;
					}

					_identityMap.SetFromLoad(_mapper.ToAggregate(document));
				}
			}
			finally
			{
				_identityMapLock.Release();
			}

			return _identityMap.Get(id);
		}

		public virtual void Add(TAggregate aggregate)
		{
			Guard.Against.Null(aggregate, nameof(aggregate));
			_identityMap.Add(aggregate);
		}

		public virtual void Delete(TAggregate aggregate)
		{
			Guard.Against.Null(aggregate, nameof(aggregate));
			_identityMap.Delete(aggregate);
		}

		public List<ICommitAggregateOperation> GetCommitOperations()
		{
			List<AggregateIdentityMap<TAggregate, TId>.Entry> entries = _identityMap.GetCommitableEntries();

			IEnumerable<ICommitAggregateOperation> replaceOperations = entries
				.Where(e => e.State == EntryState.Loaded)
				.Select(e => new AggregateReplaceOperation<TAggregate, TDocument, TId>(e.Aggregate, _mapper.ToDocument));

			IEnumerable<ICommitAggregateOperation> insertOperations = entries
				.Where(e => e.State == EntryState.Added)
				.Select(e => new AggregateInsertOperation<TAggregate, TDocument, TId>(e.Aggregate, _mapper.ToDocument));

			IEnumerable<ICommitAggregateOperation> deleteOperations = entries
				.Where(e => e.State == EntryState.Deleted)
				.Select(e => new AggregateDeleteOperation<TAggregate, TDocument, TId>(e.Aggregate));

			return replaceOperations
				.Concat(insertOperations)
				.Concat(deleteOperations)
				.ToList();
		}

		public void OnTransactionCommitOrAborted()
		{
			_identityMap.Reset();
		}
	}
}
