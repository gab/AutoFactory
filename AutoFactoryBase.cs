using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoFactory
{
    /// <summary>
    /// Base class for the AutoFactory implementations
    /// </summary>
    /// <typeparam name="TBase">The base type of the parts.</typeparam>
    internal abstract class AutoFactoryBase<TBase> : IAutoFactory<TBase>
        where TBase : class
    {
        /// <summary>
        /// Seeks a part that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <returns>`0.</returns>
        public TBase SeekPart(Func<Type, bool> predicate)
        {
            return SeekParts(predicate).SingleOrDefault();
        }
        /// <summary>
        /// Seeks a part that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <returns>`0.</returns>
        public TBase SeekPartFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute
        {
            return SeekPartsFromAttribute(predicate).SingleOrDefault();
        }
        /// <summary>
        /// Seeks parts that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <returns>IEnumerable{`0}.</returns>
        public abstract IEnumerable<TBase> SeekParts(Func<Type, bool> predicate);
        /// <summary>
        /// Seeks parts that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <returns>IEnumerable{`0}.</returns>
        public abstract IEnumerable<TBase> SeekPartsFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute;
        /// <summary>
        /// Composes the parts.
        /// </summary>
        /// <param name="assemblies">The assemblies to look into.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <param name="dependencyTypes">The dependency types.</param>
        internal abstract void ComposeParts(Assembly[] assemblies, object[] dependencies, Type[] dependencyTypes);
        /// <summary>
        /// Seeks a part that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        object IAutoFactory.SeekPart(Func<Type, bool> predicate)
        {
            return SeekPart(predicate);
        }
        /// <summary>
        /// Seeks a part that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        object IAutoFactory.SeekPartFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
        {
            return SeekPartFromAttribute(predicate);
        }
        /// <summary>
        /// Seeks parts that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        /// <exception cref="System.NotImplementedException"></exception>
        IEnumerable IAutoFactory.SeekParts(Func<Type, bool> predicate)
        {
            return SeekParts(predicate);
        }
        /// <summary>
        /// Seeks parts that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        IEnumerable IAutoFactory.SeekPartsFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
        {
            return SeekPartsFromAttribute(predicate);
        }
    }
}
