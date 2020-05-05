using AutoMapper;
using MongoDB.Driver;
using R5.MongoRepository.Core;
using R5.MongoRepository.IdentityMap;
using System.Collections.Generic;
using System.Linq;

namespace R5.MongoRepository.CommitOperations
{
	internal sealed class EntryToOperationConverter<TAggregate, TDocument, TId>
		where TAggregate : class
		where TDocument : class
	{
		private readonly IMongoCollection<TDocument> _collection;
		private readonly AggregateMapper<TAggregate, TDocument, TId> _mapper;
		private readonly FilterDefinitionResolver<TDocument, TId> _filterResolver;

		internal EntryToOperationConverter(
			IMongoCollection<TDocument> collection,
			AggregateMapper<TAggregate, TDocument, TId> mapper,
			FilterDefinitionResolver<TDocument, TId> filterResolver)
		{
			_collection = collection;
			_mapper = mapper;
			_filterResolver = filterResolver;
		}

		internal List<ICommitAggregateOperation> GetOperations(List<Entry<TAggregate>> entries)
		{
			IEnumerable<ICommitAggregateOperation> replaceOperations = entries
				.Where(e => e.State == EntryState.Loaded)
				.Select(ToReplaceOperation);

			IEnumerable<ICommitAggregateOperation> insertOperations = entries
				.Where(e => e.State == EntryState.Added)
				.Select(ToInsertOperation);

			IEnumerable<ICommitAggregateOperation> deleteOperations = entries
				.Where(e => e.State == EntryState.Deleted)
				.Select(ToDeleteOperation);

			return replaceOperations
				.Concat(insertOperations)
				.Concat(deleteOperations)
				.ToList();
		}

		private ReplaceOperation<TAggregate, TDocument, TId> ToReplaceOperation(Entry<TAggregate> e)
		{
			FilterDefinition<TDocument> filter = _filterResolver.MatchById(_mapper.GetIdFrom(e.Aggregate));
			TDocument document = _mapper.ToDocument(e.Aggregate);

			return new ReplaceOperation<TAggregate, TDocument, TId>(_collection, filter, document);
		}

		private InsertOperation<TAggregate, TDocument, TId> ToInsertOperation(Entry<TAggregate> e)
		{
			TDocument document = _mapper.ToDocument(e.Aggregate);

			return new InsertOperation<TAggregate, TDocument, TId>(_collection, document);
		}

		private DeleteOperation<TAggregate, TDocument, TId> ToDeleteOperation(Entry<TAggregate> e)
		{
			FilterDefinition<TDocument> filter = _filterResolver.MatchById(_mapper.GetIdFrom(e.Aggregate));

			return new DeleteOperation<TAggregate, TDocument, TId>(_collection, filter);
		}
	}
}
