using MongoDB.Driver;
using R5.MongoRepository.Core;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R5.MongoRepository.CommitOperations
{
	public sealed class DeleteOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : class
		where TDocument : class
	{
		private readonly TId _aggregateId;
		private readonly TAggregate _aggregate;
		private readonly Expression<Func<TDocument, TId>> _documentIdSelector;

		public DeleteOperation(
			TId aggregateId,
			TAggregate aggregate,
			Expression<Func<TDocument, TId>> documentIdSelector)
		{
			_aggregateId = aggregateId;
			_aggregate = aggregate;
			_documentIdSelector = documentIdSelector;
		}

		public void Execute(IMongoSessionContext sessionContext)
		{
			sessionContext.GetCollection<TDocument>()
				.DeleteOneAsync(Builders<TDocument>.Filter.Eq(_documentIdSelector, _aggregateId));
		}

		public Task ExecuteAsync(IMongoSessionContext sessionContext)
		{
			return sessionContext.GetCollection<TDocument>()
				.DeleteOneAsync(Builders<TDocument>.Filter.Eq(_documentIdSelector, _aggregateId));
		}
	}
}
