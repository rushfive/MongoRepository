using MongoDB.Driver;
using R5.MongoRepository.Core;
using System;
using System.Threading.Tasks;

namespace R5.MongoRepository.CommitOperations
{
	internal sealed class InsertOperation<TAggregate, TDocument, TId> : ICommitAggregateOperation
		where TAggregate : class
		where TDocument : class
	{
		private readonly IMongoCollection<TDocument> _collection;
		private readonly TDocument _document;

		internal InsertOperation(
			IMongoCollection<TDocument> collection,
			TDocument document)
		{
			_collection = collection;
			_document = document;
		}

		public Task ExecuteAsync(IClientSessionHandle session)
		{
			return _collection.InsertOneAsync(session, _document);
		}
	}
}
