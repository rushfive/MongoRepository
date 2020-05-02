using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Ardalis.GuardClauses;
using System.Linq;
using R5.MongoRepository.IdentityMap;
using System.Linq.Expressions;
using MongoDB.Driver.Linq;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using R5.MongoRepository.CommitOperations;

namespace R5.MongoRepository
{
	internal sealed class AggregateMapper<TAggregate, TDocument, TId>
		where TAggregate : class
		where TDocument : class
	{
		private readonly IMapper _mapper;
		private readonly Func<TAggregate, TId> _aggregateIdSelector;
		private readonly Func<TDocument, TId> _documentIdSelector;
		

		internal AggregateMapper(
			IMapper mapper,
			Func<TAggregate, TId> aggregateIdSelector,
			Expression<Func<TDocument, TId>> documentIdSelector)
		{
			_mapper = mapper;
			_aggregateIdSelector = aggregateIdSelector;
			_documentIdSelector = documentIdSelector.Compile();
		}

		internal TId GetIdFrom(TDocument document)
		{
			return _documentIdSelector(document);
		}

		internal TId GetIdFrom(TAggregate aggregate)
		{
			return _aggregateIdSelector(aggregate);
		}

		internal TAggregate ToAggregate(TDocument document)
		{
			return _mapper.Map<TAggregate>(document); ;
		}

		internal TDocument ToDocument(TAggregate aggregate)
		{
			return _mapper.Map<TDocument>(aggregate);
		}

		internal TTargetExpression MapExpression<TSourceExpression, TTargetExpression>(TSourceExpression expression)
			where TSourceExpression : LambdaExpression
			where TTargetExpression : LambdaExpression
		{
			return _mapper.MapExpression<TSourceExpression, TTargetExpression>(expression);
		}
	}
}
