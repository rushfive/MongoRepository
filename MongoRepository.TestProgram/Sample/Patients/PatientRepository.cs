using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.MongoRepository.TestProgram.Sample.Patients
{
	public sealed class PatientRepository : MongoRepository<Patient, PatientDocument, Guid>
	{
		public PatientRepository(IMongoSessionContext sessionContext, IAggregateMapper<Patient, PatientDocument, Guid> mapper) : base(sessionContext, mapper)
		{
		}
	}

	public sealed class PatientAggregateMapper : IAggregateMapper<Patient, PatientDocument, Guid>
	{
		public Patient ToAggregate(PatientDocument document)
		{
			throw new NotImplementedException();
		}

		public PatientDocument ToDocument(Patient aggregate)
		{
			throw new NotImplementedException();
		}
	}
}
