namespace R5.MongoRepository.Core
{
	public interface ISessionIdentityCache
	{
		void OnTransactionCommitOrAborted();
	}
}
