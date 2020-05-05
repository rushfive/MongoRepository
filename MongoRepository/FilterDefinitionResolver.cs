using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace R5.MongoRepository
{
	internal sealed class FilterDefinitionResolver<TDocument, TId>
		where TDocument : class
	{
		private readonly Expression<Func<TDocument, TId>> _documentIdSelector;

		internal FilterDefinitionResolver(
			Expression<Func<TDocument, TId>> documentIdSelector)
		{
			_documentIdSelector = documentIdSelector;
		}

		internal FilterDefinition<TDocument> MatchById(TId id)
		{
			return Builders<TDocument>.Filter.Eq(_documentIdSelector, id);
		}
	}
}
