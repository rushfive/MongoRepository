using AutoMapper;
using MongoDB.Driver;
using R5.MongoRepository.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace R5.MongoRepository
{
	public abstract class MongoRepositoryDbContextBuilder
	{
		public static MongoRepositoryDbContextBuilder<TDbContext> For<TDbContext>(IMongoDatabase database)
			where TDbContext : MongoRepositoryDbContext
				=> new MongoRepositoryDbContextBuilder<TDbContext>(database);
	}

	public class MongoRepositoryDbContextBuilder<TDbContext>
		where TDbContext : MongoRepositoryDbContext
	{
		private readonly IMongoDatabase _database;

		private readonly List<Action<IMapperConfigurationExpression>> _mapperConfigurations
			= new List<Action<IMapperConfigurationExpression>>();

		private readonly Dictionary<Type, Func<IMapper, MongoRepository>> _repositoryFactories
			= new Dictionary<Type, Func<IMapper, MongoRepository>>();

		internal MongoRepositoryDbContextBuilder(IMongoDatabase database)
		{
			_database = database;
		}

		public MongoRepositoryDbContextBuilder<TDbContext>  RegisterRepositoryFor<TAggregate, TDocument, TId>(
			Func<TAggregate, TId> aggregateIdSelector,
			Expression<Func<TDocument, TId>> documentIdSelector,
			Action<IMappingExpression<TAggregate, TDocument>> configureToDocumentMapping,
			Action<IMappingExpression<TDocument, TAggregate>> configureToAggregateMapping)
			where TAggregate : class
			where TDocument : class
		{
			var documentType = typeof(TDocument);

			var collectionAttribute = typeof(TDocument).GetTypeInfo().GetCustomAttribute<MongoCollectionAttribute>(true);
			if (collectionAttribute == null)
			{
				throw new InvalidOperationException($"Failed to resolve collection name for '{documentType.FullName}': the class is missing a '{nameof(MongoCollectionAttribute)}'");
			}

			return RegisterRepositoryFor(collectionAttribute.Name, aggregateIdSelector, documentIdSelector, configureToDocumentMapping, configureToAggregateMapping);
		}

		public MongoRepositoryDbContextBuilder<TDbContext> RegisterRepositoryFor<TAggregate, TDocument, TId>(
			string collectionName,
			Func<TAggregate, TId> aggregateIdSelector,
			Expression<Func<TDocument, TId>> documentIdSelector,
			Action<IMappingExpression<TAggregate, TDocument>> configureToDocumentMapping,
			Action<IMappingExpression<TDocument, TAggregate>> configureToAggregateMapping)
			where TAggregate : class
			where TDocument : class
		{
			if (configureToDocumentMapping != null)
			{
				Action<IMapperConfigurationExpression> configureRepositoryMapper = config =>
				{
					IMappingExpression<TAggregate, TDocument> mappingConfig = config.CreateMap<TAggregate, TDocument>();
					configureToDocumentMapping(mappingConfig);
				};

				_mapperConfigurations.Add(configureRepositoryMapper);
			}

			if (configureToAggregateMapping != null)
			{
				Action<IMapperConfigurationExpression> configureRepositoryMapper = config =>
				{
					IMappingExpression<TDocument, TAggregate> mappingConfig = config.CreateMap<TDocument, TAggregate>();
					configureToAggregateMapping(mappingConfig);
				};

				_mapperConfigurations.Add(configureRepositoryMapper);
			}

			_repositoryFactories.Add(
				typeof(TAggregate),
				mapper =>
				{
					IMongoCollection<TDocument> collection = _database.GetCollection<TDocument>(collectionName);
					return new MongoRepository<TAggregate, TDocument, TId>(collection, mapper, aggregateIdSelector, documentIdSelector);
				});

			return this;
		}

		public TDbContext Create()
		{
			var mapperConfig = new MapperConfiguration(config =>
			{
				_mapperConfigurations.ForEach(configureRepositoryMapper => configureRepositoryMapper(config));
			});

			IMapper mapper = mapperConfig.CreateMapper();
			
			Dictionary<Type, MongoRepository> repositories = _repositoryFactories.ToDictionary(
				kv => kv.Key,
				kv => kv.Value.Invoke(mapper));

			return (TDbContext)Activator.CreateInstance(typeof(TDbContext), repositories);
		}
	}
}
