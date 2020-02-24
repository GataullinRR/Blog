using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Blog.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Utilities.Extensions;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Expressions;
using Utilities.Types;
using System.Collections;

namespace Blog.Misc.ModelBinder
{

    public class TestProxy<T> : EnumeratorProxyBase<T>
    {
        public TestProxy(IEnumerator<T> @base) : base(@base)
        {

        }
    }

    public abstract class EnumeratorProxyBase<T> : IEnumerator<T>
    {
        readonly IEnumerator<T> _base;

        public T Current => _base.Current;
        object IEnumerator.Current => ((IEnumerator)_base).Current;

        protected EnumeratorProxyBase(IEnumerator<T> @base)
        {
            _base = @base ?? throw new ArgumentNullException(nameof(@base));
        }

        public virtual void Dispose()
        {
            _base.Dispose();
        }

        public virtual bool MoveNext()
        {
            return _base.MoveNext();
        }

        public virtual void Reset()
        {
            _base.Reset();
        }
    }

    public class Ddd<T> : QueryableProxyBase<T>
    {
        public Ddd(IQueryable<T> @base) : base(@base)
        {

        }

        public override IEnumerator<T> GetEnumerator()
        {
            return new TestProxy<T>(base.GetEnumerator());
        }
    }

    public class IdToEntityBinder : IModelBinder
    {
        private readonly BlogContext _context;

        public IdToEntityBinder(BlogContext context)
        {
            _context = context;
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var metadata = (DefaultModelMetadata)bindingContext.ModelMetadata;
            var idParameterName = metadata.Attributes.ParameterAttributes
                .First(a => a.GetType() == typeof(FromEntityIdAttribute))
                .To<FromEntityIdAttribute>().IdParameterName;
            
            var idValueProviderResult = bindingContext.ValueProvider.GetValue(idParameterName);
            if (idValueProviderResult == ValueProviderResult.None)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                if (metadata.IsRequired)
                {
                    bindingContext.ModelState.TryAddModelError(idValueProviderResult.ToString(), "Id property is not set");
                }
                
                return;
            }

            var value = idValueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                if (metadata.IsRequired)
                {
                    bindingContext.ModelState.TryAddModelError(idValueProviderResult.ToString(), "Id property is not set");
                }

                return;
            }

            var entityMetadata = bindingContext.ModelMetadata.ElementMetadata;
            var entityIdPropertyMetadata = bindingContext.ModelMetadata.ElementMetadata.Properties
                .Cast<DefaultModelMetadata>()
                .FirstOrDefault(m => m.Attributes.PropertyAttributes.Any(a => a.GetType() == typeof(KeyAttribute)));
            if (entityIdPropertyMetadata == null)
            {
                throw new ArgumentException("Set KeyAttribute on id property");
            }

            object entityId = value;
            var entityIdType = entityIdPropertyMetadata.UnderlyingOrModelType;
            if (entityIdType == typeof(int))
            {
                var parsed = int.TryParse(value, out var id);
                entityId = parsed ? id : (object)null;
            }
            else if (entityIdType == typeof(string))
            {

            }
            else
            {
                throw new NotSupportedException();
            }

            if (entityId == null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                if (metadata.IsRequired)
                {
                    bindingContext.ModelState.TryAddModelError(idValueProviderResult.ToString(), "Could not parse Id parameter");
                }

                return;
            }

#warning add caching
            var entityType = entityMetadata.ModelType;
            var tableDbSetType = typeof(DbSet<>).MakeGenericType(entityType);
            var tableContextDbSetInfo = typeof(BlogContext)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Single(p => p.PropertyType == tableDbSetType);
            dynamic table = tableContextDbSetInfo.GetValue(_context);
            
            var entityEx = Expression.Parameter(entityType);
            var entityIdEx= Expression.Constant(entityId, entityIdType);
            var actualEntityIdEx = Expression.Property(entityEx, entityType.GetProperty("Id"));
            var entityIdComparerEx = Expression.Equal(entityIdEx, actualEntityIdEx);
            var lambdaType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));
            var lambdaExFactoryInfo = typeof(Expression)
                .GetMethod(nameof(Expression.Lambda), 1, 
                    new Type[] 
                    { 
                        typeof(Expression), 
                        typeof(ParameterExpression[]) 
                    });
            var whereSelectorEx = lambdaExFactoryInfo
                .MakeGenericMethod(lambdaType)
                .Invoke(null,
                    new object[]
                    {
                        entityIdComparerEx,
                        new ParameterExpression[] { entityEx }
                    });
            var query = typeof(IdToEntityBinder)
                .GetMethod(nameof(createWhereQuery), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(entityType)
                .Invoke(null, new object[] { table, whereSelectorEx });
            //query = Activator.CreateInstance(typeof(Ddd<>).MakeGenericType(entityType), query);
            bindingContext.Result = ModelBindingResult.Success(query);
        }

        static IQueryable<T> createWhereQuery<T>(DbSet<T> set, Expression<Func<T, bool>> select) where T : class
        {
            return Queryable.Where(set, select);
        }
    }
}
