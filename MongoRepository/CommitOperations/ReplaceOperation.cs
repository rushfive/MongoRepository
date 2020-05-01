using MongoDB.Driver;
using R5.MongoRepository.Core;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R5.MongoRepository.CommitOperations
{
	public sealed class ReplaceOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : class
		where TDocument : class
	{
		private readonly TId _aggregateId;
		private readonly TAggregate _aggregate;
		private readonly Func<TAggregate, TDocument> _toDocument;
		private readonly Expression<Func<TDocument, TId>> _documentIdSelector;

		public ReplaceOperation(
			TId aggregateId,
			TAggregate aggregate,
			Func<TAggregate, TDocument> toDocument,
			Expression<Func<TDocument, TId>> documentIdSelector)
		{
			_aggregateId = aggregateId;
			_aggregate = aggregate;
			_toDocument = toDocument;
			_documentIdSelector = documentIdSelector;
		}

		public void Execute(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			sessionContext.GetCollection<TDocument>()
				.ReplaceOne(
					Builders<TDocument>.Filter.Eq(_documentIdSelector, _aggregateId),
					document);
		}

		public Task ExecuteAsync(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			return sessionContext.GetCollection<TDocument>()
				.ReplaceOneAsync(
					Builders<TDocument>.Filter.Eq(_documentIdSelector, _aggregateId),
					document);
		}
	}
}
