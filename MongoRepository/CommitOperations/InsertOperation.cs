using R5.MongoRepository.Core;
using System;
using System.Threading.Tasks;

namespace R5.MongoRepository.CommitOperations
{
	public sealed class InsertOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : class
		where TDocument : class
	{
		private readonly TAggregate _aggregate;
		private readonly Func<TAggregate, TDocument> _toDocument;

		public InsertOperation(
			TAggregate aggregate,
			Func<TAggregate, TDocument> toDocument)
		{
			_aggregate = aggregate;
			_toDocument = toDocument;
		}

		public void Execute(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			sessionContext.GetCollection<TDocument>().InsertOne(document);
		}

		public Task ExecuteAsync(IMongoSessionContext sessionContext)
		{
			TDocument document = _toDocument(_aggregate);

			return sessionContext.GetCollection<TDocument>().InsertOneAsync(document);
		}
	}
}
