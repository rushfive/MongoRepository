using MongoDB.Driver;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public interface ICommitAggregateOperation
	{
		Task ExecuteAsync(IClientSessionHandle session);
		//Task ExecuteAsync(IMongoSessionContext sessionContext);
		//void Execute(IMongoSessionContext sessionContext);
	}
}
