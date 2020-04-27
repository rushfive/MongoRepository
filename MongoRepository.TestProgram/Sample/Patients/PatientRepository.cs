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
			return new Patient
			{
				Id = document.Id,
				FullName = $"{document.FirstName} {document.LastName}".Trim()
			};
		}

		public PatientDocument ToDocument(Patient aggregate)
		{
			string firstName = "", lastName = "";

			if (!string.IsNullOrWhiteSpace(aggregate.FullName))
			{
				var nameSplit = aggregate.FullName.Split(' ');
				if (nameSplit.Length > 0)
					firstName = nameSplit[0];
				if (nameSplit.Length > 1)
					lastName = nameSplit[1];
			}

			return new PatientDocument
			{
				Id = aggregate.Id,
				FirstName = firstName,
				LastName = lastName
			};
			
		}
	}
}
