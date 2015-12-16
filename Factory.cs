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
        /// Creates a new factory for the type <typeparamref name="TBase"/> in the calling assembly, 
        /// </summary>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        public static IAutoFactory<TBase> Create<TBase>()
            where TBase : class
        {
            return Create<TBase>(Assembly.GetCallingAssembly(), new TypedParameter[] { });
        }
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
        /// Creates a new factory for the type <typeparamref name="TBase"/> 
        /// passing the dependency values and types (<paramref name="constructorParams"/> and <paramref name="constructorParamTypes"/>) to be injected
        /// in the constructor of the parts.
        /// </summary>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        /// <param name="assembly">The assembly containing the parts</param>
        /// <param name="constructorParams">The dependency values (constructor parameters) to inject when creating a part.</param>
        /// <param name="constructorParamTypes">The dependency types (constructor parameter types). Must be of the same size as <paramref name="constructorParams"/></param>
        public static IAutoFactory<TBase> Create<TBase>(Assembly assembly, object[] constructorParams = null, Type[] constructorParamTypes = null) where TBase : class
        {
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }
            return Create<TBase>(new[] { assembly }, constructorParams, constructorParamTypes);
        }
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase"/> 
        /// passing the dependency values and types (<paramref name="constructorParams"/> and <paramref name="constructorParamTypes"/>) to be injected
        /// in the constructor of the parts.
        /// </summary>
        /// <typeparam name="TBase">The base class/interface from which the parts derives</typeparam>
        /// <param name="assemblies">The assemblies containing the parts</param>
        /// <param name="constructorParams">The dependency values (constructor parameters) to inject when creating a part.</param>
        /// <param name="constructorParamTypes">The dependency types (constructor parameter types). Must be of the same size as <paramref name="constructorParams"/></param>
        public static IAutoFactory<TBase> Create<TBase>(Assembly[] assemblies, object[] constructorParams = null, Type[] constructorParamTypes = null) 
            where TBase : class
        {
            if (assemblies == null)
            {
                assemblies = new [] { Assembly.GetCallingAssembly() };
            }
            if (constructorParamTypes == null)
            {
                constructorParamTypes = constructorParams != null ? constructorParams.Select(o => o.GetType()).ToArray() : new Type[] { };
            }
            if (constructorParams == null)
            {
                constructorParams = new object[] { };
            }
            return CreateProc<TBase>(assemblies, 
                constructorParams.Select((o, i) => new TypedParameter(constructorParamTypes[i], o)).ToArray());
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
        /// <summary>
        /// Creates a new factory for the type <typeparamref name="TBase"/> using the given constructor parameters on <paramref name="constructorParams"/>, 
        /// </summary>
        /// <typeparam name="TBase">The type of the t base.</typeparam>
        /// <param name="constructorParams">The constructor parameters.</param>
        public static IAutoFactory<TBase> Create<TBase>(params TypedParameter[] constructorParams)
            where TBase : class
        {
            return CreateProc<TBase>(new[] { Assembly.GetCallingAssembly() }, constructorParams);
        }
        #endregion

        #region Non-Generic Interface
        /// <summary>
        /// Creates a new factory for the base type given.
        /// </summary>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        public static IAutoFactory Create(Type baseType)
        {
            return Create(baseType, Assembly.GetCallingAssembly(), new TypedParameter[] { });
        }
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
            if (assembly == null)
            {
                assembly = Assembly.GetCallingAssembly();
            }
            return Create(baseType, new[] { assembly }, constructorParams, constructorParamTypes);
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
            if (constructorParams == null)
            {
                constructorParams = new object[] { };
            }
            if (constructorParamTypes == null)
            {
                constructorParamTypes = new Type[] { };
            }
            return Create(baseType, assemblies,
                constructorParamTypes.Select((pt, i) => new TypedParameter(pt, constructorParams[i])).ToArray());
        }
        /// <summary>
        /// Creates a new factory for the base type given, passing the dependency to be injected in the constructor of the parts.
        /// </summary>
        /// <param name="baseType">The base class/interface type from which the parts derives</param>
        /// <param name="constructorParams">The dependency types and values (constructor parameters) to inject when creating a part.</param>
        public static IAutoFactory Create(Type baseType, params TypedParameter[] constructorParams)
        {
            return Create(baseType, Assembly.GetCallingAssembly(), constructorParams);
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
            var method = typeof(Factory).GetMethod("CreateProc", BindingFlags.Static | BindingFlags.NonPublic);
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
