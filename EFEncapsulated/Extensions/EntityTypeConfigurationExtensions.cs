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
        /// Configures a many relationship for an entity type
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
    }
}