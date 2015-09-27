using System;
using System.Collections.Generic;

namespace AutoFactory
{
    /// <summary>
    /// Generic Factory generic interface.
    /// </summary>
    public interface IAutoFactory<TBase> : IAutoFactory
        where TBase : class
    {
        /// <summary>
        /// Seeks a part that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        new TBase SeekPart(Func<Type, bool> predicate);
        /// <summary>
        /// Seeks a part that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        new TBase SeekPartFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute;
        /// <summary>
        /// Seeks parts that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        new IEnumerable<TBase> SeekParts(Func<Type, bool> predicate);
        /// <summary>
        /// Seeks parts that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        new IEnumerable<TBase> SeekPartsFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute;
        /// <summary>
        /// Gets a part from its type.
        /// </summary>
        /// <typeparam name="T">The part type</typeparam>
        T GetPart<T>() where T : class, TBase;
    }
}
