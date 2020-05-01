using AutoMapper;
using R5.MongoRepository.Core;
using R5.MongoRepository.IdentityMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace R5.MongoRepository.CommitOperations
{
	internal sealed class EntryToOperationConverter<TAggregate, TDocument, TId>
		where TAggregate : class
		where TDocument : class
	{
		private readonly IMapper _mapper;
		private readonly Func<TAggregate, TId> _aggregateIdSelector;
		private readonly Expression<Func<TDocument, TId>> _documentIdSelector;

		internal EntryToOperationConverter(
			IMapper mapper,
			Func<TAggregate, TId> aggregateIdSelector,
			Expression<Func<TDocument, TId>> documentIdSelector)
		{
			_mapper = mapper;
			_aggregateIdSelector = aggregateIdSelector;
			_documentIdSelector = documentIdSelector;
		}

		internal List<ICommitAggregateOperation> GetOperations(List<Entry<TAggregate>> entries)
		{
			IEnumerable<ICommitAggregateOperation> replaceOperations = entries
				.Where(e => e.State == EntryState.Loaded)
				.Select(e => new ReplaceOperation<TAggregate, TDocument, TId>(_aggregateIdSelector(e.Aggregate), e.Aggregate, _mapper.Map<TDocument>, _documentIdSelector));

			IEnumerable<ICommitAggregateOperation> insertOperations = entries
				.Where(e => e.State == EntryState.Added)
				.Select(e => new InsertOperation<TAggregate, TDocument, TId>(e.Aggregate, _mapper.Map<TDocument>));

			IEnumerable<ICommitAggregateOperation> deleteOperations = entries
				.Where(e => e.State == EntryState.Deleted)
				.Select(e => new DeleteOperation<TAggregate, TDocument, TId>(_aggregateIdSelector(e.Aggregate), e.Aggregate, _documentIdSelector));

			return replaceOperations
				.Concat(insertOperations)
				.Concat(deleteOperations)
				.ToList();
		}
	}
}
