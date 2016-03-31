using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoFactory
{
    /// <summary>
    /// Generic factory creator.
    /// Creates Generic Factories for types deriving/implementing from a base class/interface.
    /// </summary>
    public static class Factory
    {
        #region Generic Interface
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase"/> in the given assembly, 
        /// </summary>
        /// <param name="assembly">The assembly containing the parts</param>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        public static IAutoFactory<TBase> Create<TBase>(Assembly assembly)
            where TBase : class
        {
            return Create<TBase>(assembly, new TypedParameter[0]);
        }
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase"/> in the given assemblies, 
        /// </summary>
        /// <param name="assemblies">The assemblies containing the parts</param>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        public static IAutoFactory<TBase> Create<TBase>(Assembly[] assemblies)
            where TBase : class
        {
            return Create<TBase>(assemblies, new TypedParameter[0]);
        }
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase"/> in the given assembly, using the given constructor parameters on <paramref name="constructorParams"/>, 
        /// </summary>
        /// <typeparam name="TBase">The type of the t base.</typeparam>
        /// <param name="assembly">The assembly to look into.</param>
        /// <param name="constructorParams">The constructor parameters.</param>
        public static IAutoFactory<TBase> Create<TBase>(Assembly assembly, params TypedParameter[] constructorParams)
            where TBase : class
        {
            return CreateProc<TBase>(new[] { assembly }, constructorParams);
        }
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase" /> in the given assemblies, using the given constructor parameters on <paramref name="constructorParams" />,
        /// </summary>
        /// <typeparam name="TBase">The type of the t base.</typeparam>
        /// <param name="assemblies">The assemblies to look into.</param>
        /// <param name="constructorParams">The constructor parameters.</param>
        /// <returns>IAutoFactory{``0}.</returns>
        public static IAutoFactory<TBase> Create<TBase>(Assembly[] assemblies, params TypedParameter[] constructorParams)
            where TBase : class
        {
            return CreateProc<TBase>(assemblies, constructorParams);
        }
        #endregion

        #region Non-Generic Interface
        /// <summary>
        /// Creates a new factory for the base type. 
        /// </summary>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        public static IAutoFactory Create(Type baseType)
        {
            return Create(baseType, baseType.GetTypeInfo().Assembly);
        }
        /// <summary>
        /// Creates a new factory for the base type in the assembly given, 
        /// </summary>
        /// <param name="assembly">The assembly containing the parts</param>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        public static IAutoFactory Create(Type baseType, Assembly assembly)
        {
            return Create(baseType, assembly, new TypedParameter[0]);
        }
        /// <summary>
        /// Creates a new factory for the base type in the assemblies given, 
        /// </summary>
        /// <param name="assemblies">The assemblies containing the parts.</param>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        public static IAutoFactory Create(Type baseType, Assembly[] assemblies)
        {
            return Create(baseType, assemblies, new TypedParameter[0]);
        }
        /// <summary>
        /// Creates a new factory for the base type given, 
        /// passing the dependency values and types (<paramref name="constructorParams"/> and <paramref name="constructorParamTypes"/>) to be injected
        /// in the constructor of the parts.
        /// </summary>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        /// <param name="assemblies">The assemblies containing the parts. (If null, it will use the calling assembly)</param>
        /// <param name="constructorParams">The dependency values (constructor parameters) to inject when creating a part. (If null, it will use the parameterless contructor)</param>
        /// <param name="constructorParamTypes">The dependency types (constructor parameter types). Must be of the same size as <paramref name="constructorParams"/>. Can be null to use the concrete type of <paramref name="constructorParams"/></param>
        [Obsolete("Use an overload accepting a TypedParameter array")]
        public static IAutoFactory Create(Type baseType, Assembly[] assemblies, object[] constructorParams, Type[] constructorParamTypes = null)
        {
            if (constructorParams == null)
            {
                constructorParams = new object[0];
            }
            if (constructorParamTypes == null)
            {
                constructorParamTypes = new Type[0];
            }
            return Create(baseType, assemblies,
                constructorParamTypes.Select((pt, i) => new TypedParameter(pt, constructorParams[i])).ToArray());
        }
        /// <summary>
        /// Creates a new factory for the base type given, on the assembly given, passing the dependency to be injected in the constructor of the parts.
        /// </summary>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        /// <param name="assembly">The assembly to llok into.</param>
        /// <param name="constructorParams">The constructor parameters.</param>
        /// <returns>IAutoFactory.</returns>
        public static IAutoFactory Create(Type baseType, Assembly assembly, params TypedParameter[] constructorParams)
        {
            return Create(baseType, new[] { assembly }, constructorParams);
        }
        /// <summary>
        /// Creates a new factory for the base type given, on the given assemblies, passing the dependency to be injected in the constructor of the parts.
        /// </summary>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        /// <param name="assemblies">The assemblies to look into.</param>
        /// <param name="constructorParams">The constructor parameters.</param>
        public static IAutoFactory Create(Type baseType, Assembly[] assemblies, params TypedParameter[] constructorParams)
        {
            // Call the Generic CreateProc
            var method = typeof(Factory).GetTypeInfo().GetDeclaredMethod("CreateProc");
            var genericMethod = method.MakeGenericMethod(baseType);
            var factory = (IAutoFactory)genericMethod.Invoke(null, new object[] { assemblies, constructorParams });
            return factory;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Creates a new factory (private method)
        /// </summary>
        private static IAutoFactory<TBase> CreateProc<TBase>(Assembly[] assemblies, IEnumerable<TypedParameter> constructorParams)
            where TBase : class
        {
            var factory = new AutoFactoryAutofac<TBase>();
            factory.ComposeParts(assemblies, constructorParams.Select(p => new Autofac.TypedParameter(p.Type, p.Value)).ToArray());
            return factory;
        }

        #endregion
    }
}
