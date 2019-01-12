using MVCAngularTest.BusinessLogic;
using System.Web.Http;
using Unity;
using Unity.RegistrationByConvention;
using Unity.WebApi;

namespace MVCAngularTest.WebApi
{
    public static class UnityConfig
    {
        public static UnityContainer Init()
        {

            var container = new UnityContainer();
            container.RegisterTypes(
                               AllClasses.FromLoadedAssemblies(),
                               WithMappings.FromMatchingInterface,
                               WithName.TypeName,
                               WithLifetime.ContainerControlled
                       );
            BusinessLogicUnityConfig.RegisterInContainer(container);
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
            return container;
        }
    }
}