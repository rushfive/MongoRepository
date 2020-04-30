using R5.MongoRepository.Core;

namespace R5.MongoRepository
{
	public interface IAggregateMapper<TAggregate, TDocument>
		where TAggregate : class
		where TDocument : class
	{
		TDocument ToDocument(TAggregate aggregate);
		TAggregate ToAggregate(TDocument document);
	}
}
