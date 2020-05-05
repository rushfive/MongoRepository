using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public interface IUnitOfWork
	{
		Task SaveChanges(bool useTransaction = true);
	}
}
