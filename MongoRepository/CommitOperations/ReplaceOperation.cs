using MongoDB.Driver;
using R5.MongoRepository.Core;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace R5.MongoRepository.CommitOperations
{
	internal sealed class ReplaceOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : class
		where TDocument : class
	{
		private readonly IMongoCollection<TDocument> _collection;
		private readonly FilterDefinition<TDocument> _matchByIdFilter;
		private readonly TDocument _document;

		internal ReplaceOperation(
			IMongoCollection<TDocument> collection,
			FilterDefinition<TDocument> matchByIdFilter,
			TDocument document)
		{
			_collection = collection;
			_matchByIdFilter = matchByIdFilter;
			_document = document;
		}

		public Task ExecuteAsync(IClientSessionHandle session)
		{
			return _collection.ReplaceOneAsync(session, _matchByIdFilter, _document);

			//return sessionContext.GetCollection<TDocument>()
			//	.ReplaceOneAsync(
			//		Builders<TDocument>.Filter.Eq(_documentIdSelector, _aggregateId),
			//		document);
		}
	}
}
