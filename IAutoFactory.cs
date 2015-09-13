using System;
using System.Collections;

namespace AutoFactory
{
    /// <summary>
    /// Generic Factory non-generic interface.
    /// </summary>
    public interface IAutoFactory
    {
        /// <summary>
        /// Seeks a part that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        object SeekPart(Func<Type, bool> predicate);
        /// <summary>
        /// Seeks a part that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        object SeekPartFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute;
        /// <summary>
        /// Seeks parts that satisfy a predicate on the concrete type.
        /// </summary>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        IEnumerable SeekParts(Func<Type, bool> predicate);
        /// <summary>
        /// Seeks parts that satisfy a condition on a specified attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type on the concrete class.
        /// Concrete classes must have the attribute.</typeparam>
        /// <param name="predicate">Predicate function to identify the concrete type needed</param>
        IEnumerable SeekPartsFromAttribute<TAttribute>(Func<TAttribute, bool> predicate)
            where TAttribute : Attribute;        
    }
}