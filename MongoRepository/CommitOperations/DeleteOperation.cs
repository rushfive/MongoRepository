using MongoDB.Driver;
using R5.MongoRepository.Core;
using System.Threading.Tasks;

namespace R5.MongoRepository.CommitOperations
{
	internal sealed class DeleteOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : class
		where TDocument : class
	{
		private readonly IMongoCollection<TDocument> _collection;
		private readonly FilterDefinition<TDocument> _matchByIdFilter;

		internal DeleteOperation(
			IMongoCollection<TDocument> collection,
			FilterDefinition<TDocument> matchByIdFilter)
		{
			_collection = collection;
			_matchByIdFilter = matchByIdFilter;
		}

		public Task ExecuteAsync(IClientSessionHandle session)
		{
			return _collection.DeleteOneAsync(session, _matchByIdFilter);

			//return sessionContext.GetCollection<TDocument>()
			//	.DeleteOneAsync(Builders<TDocument>.Filter.Eq(_documentIdSelector, _aggregateId));
		}
	}
}
