using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public interface ICommitAggregateOperation
	{
		Task ExecuteAsync(IMongoSessionContext sessionContext);
		void Execute(IMongoSessionContext sessionContext);
	}
}
