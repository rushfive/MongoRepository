using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.MongoRepository.TestProgram.Sample.Patients
{
	public sealed class Patient : IAggregateRoot<Guid>
	{
		public Guid Id { get; set; }
	}

	[MongoCollection("Patients")]
	public sealed class PatientDocument : IAggregateDocument<Guid>
	{
		public Guid Id { get; set; }
	}

	
}
