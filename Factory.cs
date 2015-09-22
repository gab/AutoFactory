using System;
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
        /// Creates a new factory for the type <typeparamref name="TBase"/>, 
        /// passing the dependency values and types (<paramref name="constructorParams"/> and <paramref name="constructorParamTypes"/>) to be injected
        /// in the constructor of the parts.
        /// </summary>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        /// <param name="constructorParams">The dependency values (constructor parameters) to inject when creating a part.</param>
        /// <param name="constructorParamTypes">The dependency types (constructor parameter types). Must be of the same size as <paramref name="constructorParams"/></param>
        public static IAutoFactory<TBase> Create<TBase>(object[] constructorParams, Type[] constructorParamTypes)
            where TBase : class
        {
            return Create<TBase>(Assembly.GetCallingAssembly(), constructorParams, constructorParamTypes);
        }
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase"/>, 
        /// passing the dependency values (<paramref name="constructorParams"/> and using the concrete types to be injected as the contructor parameters.
        /// </summary>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        /// <param name="constructorParams">The dependency values (constructor parameters) to inject when creating a part.</param>
        public static IAutoFactory<TBase> Create<TBase>(object[] constructorParams)
            where TBase : class
        {
            return Create<TBase>(Assembly.GetCallingAssembly(), constructorParams);
        }
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase"/> using the given container type <paramref name="containerType"/>, 
        /// passing the dependency values and types (<paramref name="constructorParams"/> and <paramref name="constructorParamTypes"/>) to be injected
        /// in the constructor of the parts.
        /// </summary>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        /// <param name="assembly">The assembly containing the parts</param>
        /// <param name="constructorParams">The dependency values (constructor parameters) to inject when creating a part.</param>
        /// <param name="constructorParamTypes">The dependency types (constructor parameter types). Must be of the same size as <paramref name="constructorParams"/></param>
        public static IAutoFactory<TBase> Create<TBase>(Assembly assembly = null, object[] constructorParams = null, Type[] constructorParamTypes = null) where TBase : class
        {
            return Create<TBase>(assembly == null ? null : new[] { assembly }, constructorParams, constructorParamTypes);
        }
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase"/> using the given container type <paramref name="containerType"/>, 
        /// passing the dependency values and types (<paramref name="constructorParams"/> and <paramref name="constructorParamTypes"/>) to be injected
        /// in the constructor of the parts.
        /// </summary>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        /// <param name="assemblies">The assemblies containing the parts</param>
        /// <param name="constructorParams">The dependency values (constructor parameters) to inject when creating a part.</param>
        /// <param name="constructorParamTypes">The dependency types (constructor parameter types). Must be of the same size as <paramref name="constructorParams"/></param>
        public static IAutoFactory<TBase> Create<TBase>(Assembly[] assemblies = null, object[] constructorParams = null, Type[] constructorParamTypes = null) where TBase : class
        {
            if (assemblies == null)
            {
                assemblies = new [] { Assembly.GetCallingAssembly() };
            }
            return CreateProc<TBase>(assemblies, constructorParams, constructorParamTypes);
        }
        #endregion

        #region Non-Generic Interface
        /// <summary>
        /// Creates a new factory for the base type given, 
        /// passing the dependency values and types (<paramref name="constructorParams"/> and <paramref name="constructorParamTypes"/>) to be injected
        /// in the constructor of the parts.
        /// </summary>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        /// <param name="assembly">The assembly containing the parts. (If null, it will use the calling assembly)</param>
        /// <param name="constructorParams">The dependency values (constructor parameters) to inject when creating a part. (If null, it will use the parameterless contructor)</param>
        /// <param name="constructorParamTypes">The dependency types (constructor parameter types). Must be of the same size as <paramref name="constructorParams"/>. Can be null to use the concrete type of <paramref name="constructorParams"/></param>
        public static IAutoFactory Create(Type baseType, Assembly assembly = null, object[] constructorParams = null,
            Type[] constructorParamTypes = null)
        {
            return Create(baseType, assembly == null ? null : new[] { assembly }, constructorParams, constructorParamTypes);
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
        public static IAutoFactory Create(Type baseType, Assembly[] assemblies = null, object[] constructorParams = null, Type[] constructorParamTypes = null)
        {
            // Call the Generic CreateProc
            if (assemblies == null)
            {
                assemblies = new [] { Assembly.GetCallingAssembly() };
            }
            var method = typeof(Factory).GetMethod("CreateProc", BindingFlags.Static | BindingFlags.NonPublic);
            var genericMethod = method.MakeGenericMethod(baseType);
            var factory = (IAutoFactory)genericMethod.Invoke(null, new object[] { assemblies, constructorParams, constructorParamTypes });
            return factory;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Factory method to create and compose the generic factory
        /// </summary>
        private static IAutoFactory<TBase> CreateProc<TBase>(Assembly[] assemblies, object[] constructorParams, Type[] constructorParamTypes)
            where TBase : class
        {
            if (constructorParamTypes == null)
            {
                constructorParamTypes = constructorParams != null ? constructorParams.Select(o => o.GetType()).ToArray() : new Type[] { };
            }
            if (constructorParams == null)
            {
                constructorParams = new object[] { };
            }
            AutoFactoryBase<TBase> factory = new AutoFactoryAutofac<TBase>();
            factory.ComposeParts(assemblies, constructorParams, constructorParamTypes);
            return factory;
        }
        #endregion
    }
}
