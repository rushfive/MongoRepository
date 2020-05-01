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
	}

	public sealed class MongoRepository<TAggregate, TDocument, TId> : MongoRepository, IMongoRepository<TAggregate, TId>
		where TAggregate : class
		where TDocument : class
	{
		private readonly IMongoCollection<TDocument> _collection;
		private readonly IMapper _mapper;
		private readonly Func<TAggregate, TId> _aggregateIdSelector;
		private readonly Expression<Func<TDocument, TId>> _documentIdSelector;
		private readonly Func<TDocument, TId> _getIdFromDocument;
		private readonly AggregateIdentityMap<TAggregate, TId> _identityMap;
		private readonly EntryToOperationConverter<TAggregate, TDocument, TId> _entryToOperationConverter;

		internal MongoRepository(
			IMongoCollection<TDocument> collection,
			IMapper mapper,
			Func<TAggregate, TId> aggregateIdSelector,
			Expression<Func<TDocument, TId>> documentIdSelector)
		{
			_collection = collection;
			_mapper = mapper;
			_aggregateIdSelector = aggregateIdSelector;
			_documentIdSelector = documentIdSelector;
			_getIdFromDocument = documentIdSelector.Compile();
			_identityMap = new AggregateIdentityMap<TAggregate, TId>(aggregateIdSelector);
			_entryToOperationConverter = new EntryToOperationConverter<TAggregate, TDocument, TId>(mapper, aggregateIdSelector, documentIdSelector);
		}

		public async Task<TAggregate> FindOne(TId id)
		{
			if (!_identityMap.TryGet(id, out TAggregate aggregate))
			{
				TDocument document = await _collection
						.Find(Builders<TDocument>.Filter.Eq(_documentIdSelector, id))
						.SingleOrDefaultAsync();

				if (document == null)
				{
					return null;
				}

				aggregate = _mapper.Map<TAggregate>(document);
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

			TId aggregateId = _getIdFromDocument(document);
			if (!_identityMap.TryGet(aggregateId, out TAggregate aggregate))
			{
				aggregate = _mapper.Map<TAggregate>(document);
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
			TId aggregateId = _getIdFromDocument(document);

			if (!_identityMap.TryGet(aggregateId, out TAggregate aggregate))
			{
				aggregate = _mapper.Map<TAggregate>(document);
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

		public List<ICommitAggregateOperation> GetCommitOperations()
		{
			List<Entry<TAggregate>> entries = _identityMap.GetCommitableEntries();
			return _entryToOperationConverter.GetOperations(entries);
			//IEnumerable<ICommitAggregateOperation> replaceOperations = entries
			//	.Where(e => e.State == EntryState.Loaded)
			//	.Select(e => new ReplaceOperation<TAggregate, TDocument, TId>(_aggregateIdSelector(e.Aggregate), e.Aggregate, _mapper.Map<TDocument>, _documentIdSelector));

			//IEnumerable<ICommitAggregateOperation> insertOperations = entries
			//	.Where(e => e.State == EntryState.Added)
			//	.Select(e => new InsertOperation<TAggregate, TDocument, TId>(e.Aggregate, _mapper.Map<TDocument>));

			//IEnumerable<ICommitAggregateOperation> deleteOperations = entries
			//	.Where(e => e.State == EntryState.Deleted)
			//	.Select(e => new DeleteOperation<TAggregate, TDocument, TId>(_aggregateIdSelector(e.Aggregate), e.Aggregate, _documentIdSelector));

			//return replaceOperations
			//	.Concat(insertOperations)
			//	.Concat(deleteOperations)
			//	.ToList();
		}

		public void OnTransactionCommitOrAborted()
		{
			_identityMap.Reset();
		}
	}
}
