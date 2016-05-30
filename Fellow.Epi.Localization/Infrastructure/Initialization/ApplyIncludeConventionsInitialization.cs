using System.Collections.Generic;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Fellow.Epi.Localization.Infrastructure.Conventions;
using Fellow.Epi.Localization.Manager.Convention;

namespace Fellow.Epi.Localization.Infrastructure.Initialization
{
	[InitializableModule]
	[ModuleDependency(typeof(ServiceContainerInitialization))]
	public class ApplyIncludeConventionsInitialization : IInitializableModule
	{
		public void Initialize(InitializationEngine context)
		{
			IConventionManager conventionManager = context.Locate.Advanced.GetInstance<IConventionManager>();

			IEnumerable<IIncludeConvention> conventions = context.Locate.Advanced.GetAllInstances<IIncludeConvention>();

			foreach (IIncludeConvention convention in conventions)
				convention.Apply(conventionManager);
		}

		public void Uninitialize(InitializationEngine context)
		{
		}

		public void Preload(string[] parameters)
		{
		}
	}
}
