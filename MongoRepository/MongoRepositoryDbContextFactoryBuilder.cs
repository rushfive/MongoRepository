using Ardalis.GuardClauses;
using AutoMapper;
using MongoDB.Driver;
using R5.MongoRepository.CommitOperations;
using R5.MongoRepository.IdentityMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace R5.MongoRepository
{
	public static class MongoRepositoryDbContextFactoryBuilder
	{
		public static MongoRepositoryDbContextFactoryBuilder<TDbContext> For<TDbContext>()
			where TDbContext : MongoRepositoryDbContext
				=> new MongoRepositoryDbContextFactoryBuilder<TDbContext>();
	}

	public sealed class MongoRepositoryDbContextFactoryBuilder<TDbContext>
		where TDbContext : MongoRepositoryDbContext
	{
		private readonly List<Action<IMapperConfigurationExpression>> _mapperConfigurations
			= new List<Action<IMapperConfigurationExpression>>();

		private readonly Dictionary<Type, Func<IMapper, IMongoDatabase, MongoRepository>> _repositoryFactories
			= new Dictionary<Type, Func<IMapper, IMongoDatabase, MongoRepository>>();

		internal MongoRepositoryDbContextFactoryBuilder() { }

		public MongoRepositoryDbContextFactoryBuilder<TDbContext> RegisterRepository<TAggregate, TDocument, TId>(
			IAggregateConfiguration<TAggregate, TDocument, TId> configuration)
				where TAggregate : class
				where TDocument : class
		{
			Guard.Against.Null(configuration, nameof(configuration));

			if (!TryGetCollectionName(configuration, out string collectionName))
			{
				throw new InvalidOperationException($"Failed to resolve collection name for '{typeof(TDocument).FullName}': the class is missing a '{nameof(MongoCollectionAttribute)}'");
			}

			PrepareMapperConfigurations(configuration);
			PrepareRepositoryFactory(configuration, collectionName);

			return this;
		}

		private bool TryGetCollectionName<TAggregate, TDocument, TId>(
			IAggregateConfiguration<TAggregate, TDocument, TId> configuration, out string collectionName)
			where TAggregate : class
			where TDocument : class
		{
			collectionName = configuration.CollectionName;

			if (collectionName == null)
			{
				var collectionAttribute = typeof(TDocument).GetTypeInfo().GetCustomAttribute<MongoCollectionAttribute>(true);
				collectionName = collectionAttribute?.Name;
			}

			return collectionName != null;
		}

		private void PrepareMapperConfigurations<TAggregate, TDocument, TId>(
			IAggregateConfiguration<TAggregate, TDocument, TId> configuration)
			where TAggregate : class
			where TDocument : class
		{
			Guard.Against.Null(configuration.ToDocumentMappings, nameof(configuration.ToDocumentMappings));
			Guard.Against.Null(configuration.ToAggregateMappings, nameof(configuration.ToAggregateMappings));

			_mapperConfigurations.Add(config =>
			{
				IMappingExpression<TAggregate, TDocument> mappingConfig = config.CreateMap<TAggregate, TDocument>();
				configuration.ToDocumentMappings(mappingConfig);
			});

			_mapperConfigurations.Add(config =>
			{
				IMappingExpression<TDocument, TAggregate> mappingConfig = config.CreateMap<TDocument, TAggregate>();
				configuration.ToAggregateMappings(mappingConfig);
			});
		}

		private void PrepareRepositoryFactory<TAggregate, TDocument, TId>(
			IAggregateConfiguration<TAggregate, TDocument, TId> configuration, string collectionName)
				where TAggregate : class
				where TDocument : class
		{
			Guard.Against.Null(configuration.AggregateIdSelector, nameof(configuration.AggregateIdSelector));
			Guard.Against.Null(configuration.DocumentIdSelector, nameof(configuration.DocumentIdSelector));

			var repositoryKey = typeof(TAggregate);

			Func<IMapper, IMongoDatabase, MongoRepository> repositoryFactory
				= (mapper, database) =>
				{
					try
					{
						IMongoCollection<TDocument> collection = database.GetCollection<TDocument>(collectionName);
						var aggregateMapper = new AggregateMapper<TAggregate, TDocument, TId>(mapper, configuration.AggregateIdSelector, configuration.DocumentIdSelector);
						var filterResolver = new FilterDefinitionResolver<TDocument, TId>(configuration.DocumentIdSelector);

						return new MongoRepository<TAggregate, TDocument, TId>(
							collection,
							aggregateMapper,
							new AggregateIdentityMap<TAggregate, TId>(configuration.AggregateIdSelector),
							filterResolver,
							new EntryToOperationConverter<TAggregate, TDocument, TId>(collection, aggregateMapper, filterResolver));
					}
					catch (TypeInitializationException ex)
					{
						Console.WriteLine($"Failed to create collection for '{typeof(TDocument).FullName}'. "
							+ $"Ensure the mapping configurations for its '{nameof(IAggregateConfiguration<TAggregate, TDocument, TId>)}' are valid."
							+ Environment.NewLine
							+ ex);

						throw;
					}
					
				};

			_repositoryFactories.Add(repositoryKey, repositoryFactory);
		}

		public Func<IMongoDatabase, TDbContext> CreateFactory()
		{
			var mapperConfig = new MapperConfiguration(config =>
			{
				_mapperConfigurations.ForEach(configureRepositoryMapper => configureRepositoryMapper(config));
			});

			IMapper mapper = mapperConfig.CreateMapper();

			return database =>
			{
				Dictionary<Type, MongoRepository> repositories = _repositoryFactories.ToDictionary(
					kv => kv.Key,
					kv => kv.Value.Invoke(mapper, database));

				return (TDbContext)Activator.CreateInstance(typeof(TDbContext), repositories, database);
			};
		}
	}
}
