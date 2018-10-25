using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http.Dependencies;
using Unity;
using Unity.WebApi;

namespace Seva.Assessment.DataService
{
    public static class DataFactory
    {
        private static readonly UnityContainer Container;
        static DataFactory()
        {
            Container = new UnityContainer();
        }

        public static T GetInstance<T>()
        {
            return Container.Resolve<T>();
        }

        public static IDependencyResolver BuildUp(Action<IDataFactoryContainer> registerCustomTypes)
        {
            var dataFactoryConfig = new DataFactoryContainer(Container);
            dataFactoryConfig.RegisterTypeSingleton<ILogger, Logger>();
            registerCustomTypes(dataFactoryConfig);

            return new UnityDependencyResolver(Container);
        }

    }
}
