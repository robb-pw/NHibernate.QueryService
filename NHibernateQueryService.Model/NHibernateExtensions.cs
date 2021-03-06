using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace NHibernateQueryService.Model {
    public static class NHibernateExtensions {
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> query, params string[] properties) {
            if(query is NHibernate.Linq.NhQueryable<TEntity>) {
                NHibernate.Linq.NhQueryable<TEntity> nquery = (NHibernate.Linq.NhQueryable<TEntity>) query;
            }
            //    var queryOver = .QueryOver<TEntity>();

            //    var criteria = queryOver.UnderlyingCriteria;

            //    foreach (var property in properties) {
            //        criteria.SetFetchMode(property, FetchMode.Eager);
            //    }

            //    return queryOver;
            return query;
        }

        public static IQueryable<T> ProcessExpands<T>(this IQueryable<T> source) where T : class {
            string expandsQueryString = HttpContext.Current.Request.QueryString["$expand"];
            if (string.IsNullOrWhiteSpace(expandsQueryString)) {
                return source;
            }

            IQueryable<T> query = source;

            var expandExpressions = new List<Expression>();

            expandsQueryString.Split(',').Select(s => s.Trim()).ToList().ForEach(
                expand => expandExpressions.Add(GetExpandPropertyExpression(typeof(T), expand)));

            if(expandExpressions.Count() == 0) {
                return query;
            }
            //if(expandExpressions.Count == 1) {
            //    return query.Fetch()
            //}

            return query;
        }

        public static Expression GetExpandPropertyExpression(Type parentType, string propertyName) {
            if(string.IsNullOrWhiteSpace(propertyName)) {
                return null;
            }
            Expression expression = Expression.Parameter(parentType);
            var properties = propertyName.Split('/');
            return properties
                .Aggregate(expression, (current, property) => Expression.Property(current, propertyName));
        }

        //public static IQueryable<T> ApplyExpansions<T>(IQueryable<T> queryable, IEnumerable<string> expandPaths) {
        //    if (queryable == null) throw new DataServiceException("Query cannot be null");

        //    var nHibQuery = queryable.Provider as NHibernate.Linq.DefaultQueryProvider;
        //    if (nHibQuery == null) throw new DataServiceException("Expansion only supported on INHibernateQueryable queries");

        //    if (!expandPaths.Any()) throw new DataServiceException("Expansion Paths cannot be null");
        //    var currentQueryable = queryable;
        //    foreach (string expand in expandPaths) {
        //        // We always start with the resulting element type
        //        var currentType = currentQueryable.ElementType;
        //        var isFirstFetch = true;
        //        foreach (string seg in expand.Split('/')) {
        //            IClassMetadata metadata = Session.SessionFactory.GetClassMetadata(currentType);
        //            if (metadata == null) {
        //                throw new DataServiceException("Type not recognized as a valid type for this Context");
        //            }

        //            // Gather information about the property
        //            var propInfo = currentType.GetProperty(seg);
        //            var propType = propInfo.PropertyType;
        //            var metaPropType = metadata.GetPropertyType(seg);

        //            // When this is the first segment of a path, we have to use Fetch instead of ThenFetch
        //            var propFetchFunctionName = (isFirstFetch ? "Fetch" : "ThenFetch");

        //            // The delegateType is a type for the lambda creation to create the correct return value
        //            System.Type delegateType;

        //            if (metaPropType.IsCollectionType) {
        //                // We have to use "FetchMany" or "ThenFetchMany" when the target property is a collection
        //                propFetchFunctionName += "Many";

        //                // We only support IList<T> or something similar
        //                propType = propType.GetGenericArguments().Single();
        //                delegateType = typeof (Func<,>).MakeGenericType(currentType,
        //                                                                typeof (IEnumerable<>).MakeGenericType(propType));
        //            } else {
        //                delegateType = typeof (Func<,>).MakeGenericType(currentType, propType);
        //            }

        //            // Get the correct extension method (Fetch, FetchMany, ThenFetch, or ThenFetchMany)
        //            var fetchMethodInfo = typeof(EagerFetchingExtensionMethods).GetMethod(propFetchFunctionName,
        //                                                                              BindingFlags.Static |
        //                                                                              BindingFlags.Public |
        //                                                                              BindingFlags.InvokeMethod);
        //            var fetchMethodTypes = new List<System.Type>();
        //            fetchMethodTypes.AddRange(currentQueryable.GetType().GetGenericArguments().Take(isFirstFetch ? 1 : 2));
        //            fetchMethodTypes.Add(propType);
        //            fetchMethodInfo = fetchMethodInfo.MakeGenericMethod(fetchMethodTypes.ToArray());

        //            // Create an expression of type new delegateType(x => x.{seg.Name})
        //            var exprParam = System.Linq.Expressions.Expression.Parameter(currentType, "x");
        //            var exprProp = System.Linq.Expressions.Expression.Property(exprParam, seg);
        //            var exprLambda = System.Linq.Expressions.Expression.Lambda(delegateType, exprProp,
        //                                                                       new System.Linq.Expressions.
        //                                                                           ParameterExpression[] {exprParam});

        //            // Call the *Fetch* function
        //            var args = new object[] {currentQueryable, exprLambda};
        //            currentQueryable = (IQueryable) fetchMethodInfo.Invoke(null, args) as IQueryable<T>;

        //            currentType = propType;
        //            isFirstFetch = false;
        //        }
        //    }

        //    return currentQueryable;
        //}
    }
}