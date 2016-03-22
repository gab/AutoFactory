// *******************************************
// Generic Factory of Strategies using Autofac
// http://autofac.org/
// *******************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;

namespace AutoFactory
{
    /// <summary>
    /// Generic Factory using Autofac.
    /// </summary>
    /// <typeparam name="TBase">The base type of the parts.</typeparam>
    internal class AutoFactoryAutofac<TBase> : AutoFactoryBase<TBase> where TBase : class
    {
        #region Fields
        /// <summary>
        /// The Autofac parts container
        /// </summary>
        private IContainer _container;
        /// <summary>
        /// The lazily instantiated parts
        /// </summary>
        private IEnumerable<Meta<Lazy<TBase>>> _parts;
        /// <summary>
        /// The the metadata key to store the part type
        /// </summary>
        private const string MetadataKey = "PartType";
        #endregion

        #region Private Methods
        /// <summary>
        /// Try to resolve the part and return it
        /// </summary>
        /// <param name="part">The part.</param>
        private TBase TryResolve(Meta<Lazy<TBase>> part)
        {
            if (part == null)
            {
                throw new AutoFactoryException(string.Format("Factory resolution failed for type {0}", typeof(TBase).FullName));
            }
            try
            {
                return part.Value.Value;
            }
            catch (Exception ex)
            {
                throw new AutoFactoryException(string.Format("Factory resolution failed for type {0}. See inner exception for details.", typeof(TBase).FullName), ex);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Compose parts using Autofac container.
        /// </summary>
        /// <param name="assemblies">The assemblies to look into.</param>
        /// <param name="dependencies">The dependency values to inject to the part constructor</param>
        internal override void ComposeParts(Assembly[] assemblies, Autofac.TypedParameter[] dependencies)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => typeof (TBase).IsAssignableFrom(t))
                .As<TBase>()
                .WithMetadata(MetadataKey, t => t)
                .FindConstructorsWith(t => new[] {t.GetConstructor(dependencies.Select(d => d.Type).ToArray())});
            _container = builder.Build();
            _parts = _container.Resolve<IEnumerable<Meta<Lazy<TBase>>>>((IEnumerable<Autofac.TypedParameter>)dependencies);
        }
        /// <summary>
        /// Seeks the parts that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <returns>IEnumerable{`0}.</returns>
        public override IEnumerable<TBase> SeekParts(Func<Type, bool> predicate)
        {
            return _parts.Where(f => predicate(f.Metadata[MetadataKey] as Type))
                                  .Select(TryResolve);
        }
        /// <summary>
        /// Seeks the parts that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <returns>IEnumerable{`0}.</returns>
        public override IEnumerable<TBase> SeekPartsFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
        {
            foreach (var p in _parts)
            {
                var attributes = (p.Metadata[MetadataKey] as Type).GetCustomAttributes<TAttribute>();
                if (attributes != null)
                {
                    foreach (var attr in attributes.Where(predicate))
                    {
                        yield return TryResolve(p);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the discovered part types without instanting any part.
        /// </summary>
        public override Type[] GetPartTypes()
        {
            return _parts.Select(p => p.Metadata[MetadataKey] as Type).ToArray();
        }
        #endregion
    }
}
