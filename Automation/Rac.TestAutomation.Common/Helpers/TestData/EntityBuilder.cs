using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rac.TestAutomation.Common
{
    public interface IEntityBuilder<out T> where T : class
    {
        T Build();
    }

    public abstract class EntityBuilder<TEntity, TBuilder> : IEntityBuilder<TEntity>
        where TEntity : class
        where TBuilder : class, IEntityBuilder<TEntity>
    {
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

        public TEntity Build()
        {
            return BuildEntity();
        }

        private string GetPropertyName<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            var memExp = property.Body as MemberExpression ??
                         (property.Body as UnaryExpression)?.Operand as MemberExpression;

            if (memExp == null)
            {
                throw new ArgumentException("Action must be a member expression.");
            }

            if (memExp == null)
            {
                throw new ArgumentException("Property must be valid on the entity object", nameof(property));
            }

            return memExp.Member.Name;
        }

        public void Set<TValue>(Expression<Func<TEntity, TValue>> property, TValue value)
        {
            _properties[GetPropertyName(property)] = value;
        }

        public TEntity SetValue<TValue>(Expression<Func<TEntity, TValue>> property, TValue value)
        {
            Set(property, value);

            return this as TEntity;
        }

        public TValue Get<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            if (!Has(property))
            {
                throw new ArgumentException($"{property} has not been specified", nameof(property));
            }

            return (TValue)_properties[GetPropertyName(property)];
        }

        public TValue GetOrDefault<TValue>(Expression<Func<TEntity, TValue>> property, TValue defaultValue = default)
        {
            try
            {
                return Has(property) ? Get(property) : defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        protected bool Has<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            return _properties.ContainsKey(GetPropertyName(property)) && _properties[GetPropertyName(property)] != null;
        }

        protected abstract TEntity BuildEntity();

        public EntityBuilder<TEntity, TBuilder> With<TValue>(Expression<Func<TEntity, TValue>> property, TValue value)
        {
            Set(property, value);
            return this;
        }
    }
}
