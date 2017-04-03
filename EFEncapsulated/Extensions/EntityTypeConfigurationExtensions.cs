using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EFEncapsulated.Extensions
{
    public static class EntityTypeConfigurationExtensions
    {
        /// <summary>
        /// Configures a many relationship for an entity type by providing the property name explicitly
        /// </summary>
        /// <typeparam name="TEntity">The entity type to whom the collection belongs</typeparam>
        /// <typeparam name="TCollectionOfEntity">The entity type contained in the private collection</typeparam>
        /// <param name="configuration">Entity type configuration</param>
        /// <param name="collectionPropertyName">The name of the collection property</param>        
        public static ManyNavigationPropertyConfiguration<TEntity, TCollectionOfEntity> HasMany<TEntity, TCollectionOfEntity>(
            this EntityTypeConfiguration<TEntity> configuration, string collectionPropertyName)
            where TEntity : class
            where TCollectionOfEntity : class
        {
            var lambdaExpression = (Expression<Func<TEntity, ICollection<TCollectionOfEntity>>>)
                GetPropertyLambdaExpression<TEntity>(collectionPropertyName);

            return configuration.HasMany(lambdaExpression);
        }

        /// <summary>
        /// Configures a many relationship for an entity type by providing the readonly collection property
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TCollectionOfEntity"></typeparam>
        /// <param name="configuration"></param>
        /// <param name="readOnlyCollectionProperty"></param>
        /// <returns></returns>
        public static ManyNavigationPropertyConfiguration<TEntity, TCollectionOfEntity> HasMany<TEntity, TCollectionOfEntity>(
            this EntityTypeConfiguration<TEntity> configuration, Expression<Func<TEntity, IEnumerable<TCollectionOfEntity>>> readOnlyCollectionProperty)
            where TEntity : class
            where TCollectionOfEntity : class
        {
            var memberExpression = (MemberExpression)readOnlyCollectionProperty.Body;
            var readOnlyCollectionPropertyName = memberExpression.Member.Name;

            var lambdaExpression = (Expression<Func<TEntity, ICollection<TCollectionOfEntity>>>)
                GetBackingPropertyLambdaExpressionForReadOnlyProperty<TEntity>(readOnlyCollectionPropertyName);

            return configuration.HasMany(lambdaExpression);
        }

        /// <summary>
        /// Returns the lambda expression for propertyName on TEntity
        /// </summary>
        /// <typeparam name="TEntity">The entity to whom the property belongs</typeparam>
        /// <param name="propertyName">The name of the property</param>        
        public static LambdaExpression GetPropertyLambdaExpression<TEntity>(string propertyName)
            where TEntity : class
        {
            var propertyInfo = typeof(TEntity).GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (propertyInfo == null)
                throw new InvalidOperationException(
                    string.Format("Property '{0}' not found on type '{1}'", propertyName, typeof(TEntity).ToString()));

            var parameter = Expression.Parameter(typeof(TEntity), propertyName);
            var expression = Expression.Property(parameter, propertyInfo);

            return Expression.Lambda(expression, parameter);
        }

        /// <summary>
        /// Find the backing property of a readOnly property and return its lambda expression
        /// </summary>
        /// <typeparam name="TEntity">The entity to whom the property belongs</typeparam>
        /// <param name="readOnlyPropertyName">The name of the readOnly property</param>
        public static LambdaExpression GetBackingPropertyLambdaExpressionForReadOnlyProperty<TEntity>(string readOnlyPropertyName)
            where TEntity : class
        {
            var entityProperties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (!entityProperties.Any(a => a.Name == readOnlyPropertyName && a.CanRead && !a.CanWrite))
                throw new InvalidOperationException(
                    string.Format("ReadOnly property '{0}' not found on type '{1}'", readOnlyPropertyName, typeof(TEntity).ToString()));

            var lowerCase = char.ToLower(readOnlyPropertyName[0]) + readOnlyPropertyName.Substring(1);
            var lowerCaseWithUnderscore = "_" + lowerCase;
            var upperCaseWithUnderscore = "_" + char.ToUpper(readOnlyPropertyName[0]) + readOnlyPropertyName.Substring(1);

            var nameVariants = new List<string>()
            {
                lowerCase,
                lowerCaseWithUnderscore,
                upperCaseWithUnderscore
            };            

            var backingPropertyInfo = entityProperties.Where(a => nameVariants.Contains(a.Name));

            if (!backingPropertyInfo.Any())
            {
                var exceptionMessage = string.Format(
                    "Unable to find a backing property for property '{0}' on type '{1}'. Attempted: '{2}'", 
                    readOnlyPropertyName, 
                    typeof(TEntity).ToString(), 
                    string.Join("', '", nameVariants));

                throw new InvalidOperationException(exceptionMessage);
            }

            if (backingPropertyInfo.Count() > 1)
            {
                var exceptionMessage = string.Format(
                    "Unable to determine backing property for property '{0}' on type '{1}'. Ambiguous property names available: '{2}'",
                    readOnlyPropertyName,
                    typeof(TEntity).ToString(),
                    string.Join("', '", backingPropertyInfo.Select(a => a.Name)));

                throw new InvalidOperationException(exceptionMessage);
            }

            var backingPropertyName = backingPropertyInfo.Select(a => a.Name).Single();

            return GetPropertyLambdaExpression<TEntity>(backingPropertyName);
        }
    }
}