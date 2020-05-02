using AutoMapper;
using R5.MongoRepository.CommitOperations;
using System;
using System.Linq.Expressions;

namespace R5.MongoRepository
{
	public interface IAggregateConfiguration<TAggregate, TDocument, TId>
		where TAggregate : class
		where TDocument : class
	{
		string CollectionName { get; }

		Func<TAggregate, TId> AggregateIdSelector { get; }
		Expression<Func<TDocument, TId>> DocumentIdSelector { get; }

		Action<IMappingExpression<TAggregate, TDocument>> ToDocumentMappings { get; }
		Action<IMappingExpression<TDocument, TAggregate>> ToAggregateMappings { get; }

		internal EntryToOperationConverter<TAggregate, TDocument, TId> CreateEntryToOperationConverter(IMapper mapper)
		{
			return new EntryToOperationConverter<TAggregate, TDocument, TId>(mapper, AggregateIdSelector, DocumentIdSelector);
		}
	}
}
