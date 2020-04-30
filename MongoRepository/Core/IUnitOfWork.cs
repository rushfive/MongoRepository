using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository.Core
{
	public interface IUnitOfWork
	{
		Task Commit();
		Task Abort();
	}
}
