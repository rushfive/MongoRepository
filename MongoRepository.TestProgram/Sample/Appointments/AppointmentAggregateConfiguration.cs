using AutoMapper;
using R5.MongoRepository.TestProgram.Sample.Patients;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace R5.MongoRepository.TestProgram.Sample.Appointments
{
	public sealed class AppointmentAggregateConfiguration
		: IAggregateConfiguration<Appointment, AppointmentDocument, Guid>
	{
		public string CollectionName => "Appointments";

		public Func<Appointment, Guid> AggregateIdSelector => a => a.Id;
		public Expression<Func<AppointmentDocument, Guid>> DocumentIdSelector => d => d.Id;

		public Action<IMappingExpression<Appointment, AppointmentDocument>> ToDocumentMappings
			=> config =>
			{
				config.ForMember(d => d.Id, c => c.MapFrom(a => a.Id));
			};

		public Action<IMappingExpression<AppointmentDocument, Appointment>> ToAggregateMappings
			=> config =>
			{
				config.ForMember(d => d.Id, c => c.MapFrom(a => a.Id));
			};
	}

	public sealed class PatientAggregateConfiguration
		: IAggregateConfiguration<Patient, PatientDocument, Guid>
	{
		public string CollectionName => "Patients";

		public Func<Patient, Guid> AggregateIdSelector => a => a.Id;
		public Expression<Func<PatientDocument, Guid>> DocumentIdSelector => d => d.Id;

		public Action<IMappingExpression<Patient, PatientDocument>> ToDocumentMappings
			=> config =>
			{
				config
					.ForMember(d => d.Id, c => c.MapFrom(a => a.Id))
					.ForMember(d => d.FirstName, c => c.MapFrom(a => a.FullName.Split(' ', StringSplitOptions.None)[0]))
					.ForMember(d => d.LastName, c => c.MapFrom(a => a.FullName.Split(' ', StringSplitOptions.None)[1]));
			};

		public Action<IMappingExpression<PatientDocument, Patient>> ToAggregateMappings
			=> config =>
			{
				config
					.ForMember(d => d.Id, c => c.MapFrom(a => a.Id))
					.ForMember(d => d.FullName, c => c.MapFrom(a => $"{a.FirstName} {a.LastName}".Trim()));
			};
	}
}
