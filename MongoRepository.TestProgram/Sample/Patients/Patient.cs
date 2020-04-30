using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.MongoRepository.TestProgram.Sample.Patients
{
	public sealed class Patient// : IAggregateRoot<Guid>
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
	}

	[MongoCollection("Patients")]
	public sealed class PatientDocument// : IAggregateDocument<Guid>
	{
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string SessionLock { get; set; }
	}

	
}
