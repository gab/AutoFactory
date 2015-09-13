// *******************************************
// Generic Factory of Strategies using Autofac
// http://autofac.org/
// *******************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Features.Metadata;

namespace AutoFactory
{
    /// <summary>
    /// Generic Factory using Autofac.
    /// </summary>
    /// <typeparam name="TBase">The base type of the parts.</typeparam>
    /// <remarks>
    /// No limitations, this is the recommended container.
    /// </remarks>
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
            return part.Value.Value;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Compose parts using Autofac container.
        /// </summary>
        /// <param name="assembly">The assembly with the parts (when codeBase is null)</param>
        /// <param name="dependencies">The dependency values to inject to the part constructor</param>
        /// <param name="dependencyTypes">The dependency impoty types to inject to the part constructor</param>
        internal override void ComposeParts(Assembly assembly, object[] dependencies, Type[] dependencyTypes)
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => typeof(TBase).IsAssignableFrom(t))
                .As<TBase>()
                .WithMetadata(MetadataKey, t => t);
            _container = builder.Build();
            _parts = _container.Resolve<IEnumerable<Meta<Lazy<TBase>>>>
            (
                dependencyTypes.Select((t, i) => new TypedParameter(t, dependencies[i]))
            );
        }
        /// <summary>
        /// Seeks a part that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <returns>IEnumerable{`0}.</returns>
        public override IEnumerable<TBase> SeekParts(Func<Type, bool> predicate)
        {
            var partsFound = _parts.Where(f => predicate(f.Metadata[MetadataKey] as Type))
                                  .Select(TryResolve);
            return partsFound;
        }
        /// <summary>
        /// Seeks a part that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <returns>IEnumerable{`0}.</returns>
        public override IEnumerable<TBase> SeekPartsFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
        {
            var partsFound = from p in _parts
                             let attr = (p.Metadata[MetadataKey] as Type).GetCustomAttribute<TAttribute>()
                             where attr != null && predicate(attr)
                             select TryResolve(p);
            return partsFound;
        }
        #endregion
    }
}
