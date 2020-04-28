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
	public sealed class SampleMongoDbContext : MongoSessionDbContext
	{
		public IRepository<Patient, Guid> Patients { get; }
		public IRepository<Appointment, Guid> Appointments { get; }
		//private readonly MongoSessionContext _sessionContext;

		public SampleMongoDbContext(IMongoDatabase database)
			: base(database, null)
		{
			//_sessionContext = new MongoSessionContext(database);
			Patients = new PatientRepository(_sessionContext, new PatientAggregateMapper());
			Appointments = new AppointmentRepository(_sessionContext, new AppointmentAggregateMapper());

			Action onTransactionEnd = () =>
			{
				Console.WriteLine("Commited WHOO!");
				Patients.OnTransactionCommitOrAborted();
				Appointments.OnTransactionCommitOrAborted();
			};

			_onCommitCallback = onTransactionEnd;
			_onAbortCallback = onTransactionEnd;
		}

		protected override List<IAggregateOperationStore> GetOperationStores()
			=> new List<IAggregateOperationStore>
				{
					Patients, Appointments
				};
	}
}
