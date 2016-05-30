using EPiServer.Shell;
using Fellow.Epi.Localization.Infrastructure.Editor.Content.Definition;
using Fellow.Epi.Localization.Manager.Convention;
using Fellow.Epi.Localization.Manager.LocalizationConfiguration;
using Fellow.Epi.Localization.Manager.Resource;
using Fellow.Epi.Localization.Manager.Translation;
using Fellow.Epi.Localization.Repository.Resource;
using Fellow.Epi.Localization.Repository.Translation;
using StructureMap.Configuration.DSL;

namespace Fellow.Epi.Localization.Bootstrapper
{
	public class FellowEpiLocalizationImplementationBootstrapper : Registry
	{
		public FellowEpiLocalizationImplementationBootstrapper()
		{
			//Manager
			this.For<ITranslationManager>().Use<TranslationManagerCachingProxy>();
			this.For<ILocalizationConfigurationManager>().Use<LocalizationConfigurationManager>();
			this.For<IResourceManager>().Use<ResourceManagerCachingProxy>();
			this.For<IConventionManager>().Use<ConventionManager>();

			//Repository
			this.For<ITranslationRepository>().Use<TranslationRepositoryCachingProxy>();
			this.For<IResourceRepository>().Add<XmlResourceRepository>().Singleton();

			//EPiServer Overrides
			this.For<IContentRepositoryDescriptor>().Add<TranslationDataRepositoryDescriptor>();
		}
	}
}
