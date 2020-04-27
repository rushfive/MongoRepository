using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace R5.MongoRepository.TestProgram.Sample.Appointments
{
	public sealed class Appointment : IAggregateRoot<Guid>
	{
		public Guid Id { get; set; }
	}

	[MongoCollection("Appointments")]
	public sealed class AppointmentDocument : IAggregateDocument<Guid>
	{
		[BsonId]
		[BsonRepresentation(BsonType.String)]
		public Guid Id { get; set; }
	}
}
