//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;

//namespace R5.MongoRepository.Core
//{
//	public static class CollectionNameMap
//	{
//		private static Dictionary<Type, string> _typeNameMap = new Dictionary<Type, string>();

//		public static string For<TDocument>()
//		{
//			var documentType = typeof(TDocument);

//			if (_typeNameMap.TryGetValue(documentType, out string name))
//			{
//				return name;
//			}

//			var attribute = documentType.GetTypeInfo().GetCustomAttribute<MongoCollectionAttribute>(true);
//			if (attribute?.Name == null)
//			{
//				throw new InvalidOperationException($"Type '{documentType.FullName}' doesn't represent a mongo collection.");
//			}

//			_typeNameMap[documentType] = attribute.Name;
//			return attribute.Name;
//		}
//	}
//}
