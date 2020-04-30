//using AutoMapper;
//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;

//namespace R5.MongoRepository.Utilities
//{
//	// https://stackoverflow.com/questions/11248585/how-to-map-two-expressions-of-differing-types

//    public class AutoMapVisitor<TSource, TDestination> : ExpressionVisitor
//    {
//        private readonly ParameterExpression _newParameter;
        
//        private readonly TypeMap _typeMap = Mapper.FindTypeMapFor<TSource, TDestination>();

//        public AutoMapVisitor(ParameterExpression newParameter)
//        {
//            var config = new MapperConfiguration(cfg => {
//                cfg.AddProfile<AppProfile>();
//                cfg.CreateMap<Source, Dest>();
//            });

//            var mapper = config.CreateMapper();

//            _newParameter = newParameter;
//        }

//        protected override Expression VisitMember(MemberExpression node)
//        {
//            var propertyMaps = _typeMap.GetPropertyMaps();

//            // Find any mapping for this member
//            var propertyMap = propertyMaps.SingleOrDefault(map => map.SourceMember == node.Member);
//            if (propertyMap == null)
//                return base.VisitMember(node);

//            var destinationProperty = propertyMap.DestinationProperty;
//            var destinationMember = destinationProperty.MemberInfo;

//            // Check the new member is a property too
//            var property = destinationMember as PropertyInfo;
//            if (property == null)
//                return base.VisitMember(node);

//            // Access the new property
//            var newPropertyAccess = Expression.Property(_newParameter, property);
//            return base.VisitMember(newPropertyAccess);
//        }
//    }
//}
