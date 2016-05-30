using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Fellow.Epi.Localization.Bootstrapper;
using StructureMap;

namespace Fellow.Epi.Localization.Infrastructure.Initialization
{
	[InitializableModule]
	[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
	public class DependencyResolverInitialization : IConfigurableModule
	{
		public void ConfigureContainer(ServiceConfigurationContext context)
		{
			context.Container.Configure(ConfigureContainer);
		}

		private static void ConfigureContainer(ConfigurationExpression container)
		{
			//Register Authentication
			container.AddRegistry<FellowEpiLocalizationImplementationBootstrapper>();

		}

		public void Initialize(InitializationEngine context)
		{
		}

		public void Uninitialize(InitializationEngine context)
		{
		}

		public void Preload(string[] parameters)
		{
		}
	}
}
