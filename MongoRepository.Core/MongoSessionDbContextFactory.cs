using System;
using System.Collections.Generic;
using System.Text;

namespace R5.MongoRepository.Core
{
	public interface IMongoSessionDbContextFactory
	{

	}

	public sealed class MongoSessionDbContextFactory : IMongoSessionDbContextFactory
	{

	}

	public class MongoSessionDbContextOptions
	{
		public readonly CommitExecutionType CommitExecution = CommitExecutionType.Explicitly;
		public readonly RetryCommitStrategy RetryCommit = RetryCommitStrategy.None;

		public MongoSessionDbContextOptions(
			CommitExecutionType commitExecutionType,
			RetryCommitStrategy retryCommitStrategy)
		{
			CommitExecution = commitExecutionType;
			RetryCommit = retryCommitStrategy;
		}

		public MongoSessionDbContextOptions() { }

		public enum RetryCommitStrategy
		{
			None
		}

		public enum CommitExecutionType
		{
			Explicitly,
			ImplicitlyInvokedOnDisposed
		}
	}
}
