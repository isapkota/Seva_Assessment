using FluentNHibernate.Cfg;
using NHibernate;
using Seva.Assessment.DataService.User;
using Seva.Assessment.FluentNHibernetImpl;
using System;
using System.Collections.Generic;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Seva.Assessment.DataService
{
    public interface IDataFactoryContainer
    {
        /// <summary>
        /// Registers the session factory.
        /// </summary>
        /// <typeparam name="T">the type of the main entity</typeparam>
        /// <param name="environment">The identifier-string of the environment</param>
        void RegisterSessionFactory<T>(string environment);
        /// <summary>
        /// Registers the type as regular type.
        /// A new instance is created every time the type is requested.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        void RegisterType<TFrom, TTo>() where TTo : TFrom;
        /// <summary>
        /// Registers the type as singleton.
        /// The same instance is used every time the type is requested.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        void RegisterTypeSingleton<TFrom, TTo>() where TTo : TFrom;
        /// <summary>
        /// Registers the given instance for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        void RegisterInstance(Type type, object instance);
    }
    class DataFactoryContainer : IDataFactoryContainer
    {
        private readonly IUnityContainer _container;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AgriportFactoryContainer"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public DataFactoryContainer(IUnityContainer container)
        {
            _container = container;            
        }

        /// <summary>
        /// Registers the session factory.
        /// </summary>
        /// <typeparam name="T">the type of the main entity</typeparam>
        /// <param name="environment">The identifier-string of the environment</param>
        public void RegisterSessionFactory<T>(string connectionString)
        {
            _container.RegisterType<ISessionFactory>(
                new ContainerControlledLifetimeManager(), new InjectionFactory(x=>Fluently.Configure()
                .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012
                    .ConnectionString(connectionString)
                    .ShowSql()
                )
                .ExposeConfiguration(y => { y.SetInterceptor(new SqlStatementInterceptor()); })
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<UserData>()
                )
                .BuildSessionFactory()));
        }

        /// <summary>
        /// Registers the type as regular type.
        /// A new instance is created every time the type is requested.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            _container.RegisterType<TFrom, TTo>(new TransientLifetimeManager());
        }

        /// <summary>
        /// Registers the type as singleton.
        /// The same instance is used every time the type is requested.
        /// </summary>
        /// <typeparam name="TFrom">The type of from.</typeparam>
        /// <typeparam name="TTo">The type of to.</typeparam>
        public void RegisterTypeSingleton<TFrom, TTo>() where TTo : TFrom
        {
            _container.RegisterType<TFrom, TTo>(new HierarchicalLifetimeManager());
        }

        /// <summary>
        /// Registers the given instance for the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterInstance(Type type, object instance)
        {
            _container.RegisterInstance(type, instance);
        }
    }

}
