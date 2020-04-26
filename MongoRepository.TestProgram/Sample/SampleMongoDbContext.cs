using MongoDB.Driver;
using R5.MongoRepository.Core;
using R5.MongoRepository.TestProgram.Sample.Appointments;
using R5.MongoRepository.TestProgram.Sample.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R5.MongoRepository.TestProgram.Sample
{
	public interface ISampleMongoDbContext : IUnitOfWork
	{
		IRepository<Patient, Guid> Patients { get; }
		IRepository<Appointment, Guid> Appointments { get; }
	}

	public sealed class SampleMongoDbContext : ISampleMongoDbContext
	{
		public IRepository<Patient, Guid> Patients { get; }
		public IRepository<Appointment, Guid> Appointments { get; }
		private readonly MongoSessionContext _sessionContext;


		public SampleMongoDbContext(IMongoDatabase database)
		{
			_sessionContext = new MongoSessionContext(database);
			Patients = new PatientRepository(_sessionContext, new PatientAggregateMapper());
			Appointments = new AppointmentRepository(_sessionContext, new AppointmentAggregateMapper());
		}

		public async Task Commit()
		{
			var operationStores = new List<IAggregateOperationStore>
			{
				Patients, Appointments
			};

			List<ICommitAggregateOperation> operations = operationStores.SelectMany(s => s.GetCommitOperations()).ToList();

			foreach(var operation in operations)
			{
				await operation.Execute(_sessionContext);
			}

			await _sessionContext.CommitTransaction();
		}

		public Task Abort() => _sessionContext.AbortTransaction();
	}
}
