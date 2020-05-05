using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Ardalis.GuardClauses;
using System.Linq;
using R5.MongoRepository.IdentityMap;
using System.Linq.Expressions;
using MongoDB.Driver.Linq;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using R5.MongoRepository.CommitOperations;

namespace R5.MongoRepository
{
	public abstract class MongoRepository
	{
		internal MongoRepository() { }
		internal abstract List<ICommitAggregateOperation> GetCommitOperations();
		internal abstract void ClearCachedAggregateStates();
	}

	public sealed class MongoRepository<TAggregate, TDocument, TId> : MongoRepository, IMongoRepository<TAggregate, TId>
		where TAggregate : class
		where TDocument : class
	{
		private readonly IMongoCollection<TDocument> _collection;
		private readonly AggregateMapper<TAggregate, TDocument, TId> _mapper;
		private readonly AggregateIdentityMap<TAggregate, TId> _identityMap;
		private readonly FilterDefinitionResolver<TDocument, TId> _filterResolver;
		private readonly EntryToOperationConverter<TAggregate, TDocument, TId> _entryToOperationConverter;

		internal MongoRepository(
			IMongoCollection<TDocument> collection,
			AggregateMapper<TAggregate, TDocument, TId> mapper,
			AggregateIdentityMap<TAggregate, TId> identityMap,
			FilterDefinitionResolver<TDocument, TId> filterResolver,
			EntryToOperationConverter<TAggregate, TDocument, TId> entryToOperationConverter)
		{
			_collection = collection;
			_mapper = mapper;
			_identityMap = identityMap;
			_filterResolver = filterResolver;
			_entryToOperationConverter = entryToOperationConverter;
		}

		public async Task<TAggregate> FindOne(TId id)
		{
			if (!_identityMap.TryGet(id, out TAggregate aggregate))
			{
				FilterDefinition<TDocument> filter = _filterResolver.MatchById(id);

				TDocument document = await _collection
						.Find(filter)
						.SingleOrDefaultAsync();

				if (document == null)
				{
					return null;
				}

				aggregate = _mapper.ToAggregate(document);
				_identityMap.SetFromLoad(aggregate);
			}

			return aggregate;
		}

		public async Task<TAggregate> FindOneWhere(Expression<Func<TAggregate, bool>> predicate)
		{
			Expression<Func<TDocument, bool>> mappedPredicate = _mapper.MapExpression<Expression<Func<TAggregate, bool>>, Expression<Func<TDocument, bool>>>(predicate);

			IMongoQueryable<TDocument> queryable = _collection.AsQueryable();
			TDocument document = await queryable.SingleOrDefaultAsync(mappedPredicate);
			if (document == null)
			{
				return null;
			}

			TId aggregateId = _mapper.GetIdFrom(document);
			if (!_identityMap.TryGet(aggregateId, out TAggregate aggregate))
			{
				aggregate = _mapper.ToAggregate(document);
				_identityMap.SetFromLoad(aggregate);
			}

			return aggregate;
		}

		public async Task<IReadOnlyCollection<TAggregate>> Query(Expression<Func<TAggregate, bool>> predicate)
		{
			Expression<Func<TDocument, bool>> mappedPredicate = _mapper.MapExpression<Expression<Func<TAggregate, bool>>, Expression<Func<TDocument, bool>>>(predicate);

			IMongoQueryable<TDocument> queryable = _collection.AsQueryable();
			List<TDocument> documents = await queryable.Where(mappedPredicate).ToListAsync();

			return documents.Select(GetOrMapNewAggregate).ToList();
		}

		private TAggregate GetOrMapNewAggregate(TDocument document)
		{
			TId aggregateId = _mapper.GetIdFrom(document);

			if (!_identityMap.TryGet(aggregateId, out TAggregate aggregate))
			{
				aggregate = _mapper.ToAggregate(document);
				_identityMap.SetFromLoad(aggregate);
			}

			return aggregate;
		}

		public void Add(TAggregate aggregate)
		{
			Guard.Against.Null(aggregate, nameof(aggregate));
			_identityMap.Add(aggregate);
		}

		public void Delete(TAggregate aggregate)
		{
			Guard.Against.Null(aggregate, nameof(aggregate));
			_identityMap.Delete(aggregate);
		}

		internal override List<ICommitAggregateOperation> GetCommitOperations()
		{
			List<Entry<TAggregate>> entries = _identityMap.GetCommitableEntries();
			return _entryToOperationConverter.GetOperations(entries);
		}

		internal override void ClearCachedAggregateStates()
		{
			_identityMap.Reset();
		}
	}
}
