//using R5.MongoRepository.Core;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace R5.MongoRepository.TestProgram.Sample.Appointments
//{
//	public sealed class AppointmentRepository : MongoRepository<Appointment, AppointmentDocument, Guid>
//	{
//		public AppointmentRepository(IMongoSessionContext sessionContext, IAggregateMapper<Appointment, AppointmentDocument> mapper) : base(sessionContext, mapper)
//		{
//		}
//	}

//	public sealed class AppointmentAggregateMapper : IAggregateMapper<Appointment, AppointmentDocument>
//	{
//		public Appointment ToAggregate(AppointmentDocument document)
//		{
//			throw new NotImplementedException();
//		}

//		public AppointmentDocument ToDocument(Appointment aggregate)
//		{
//			throw new NotImplementedException();
//		}
//	}
//}
