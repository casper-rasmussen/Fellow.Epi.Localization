using System.Collections.Specialized;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using Fellow.Epi.Localization.Infrastructure.Editor.Content;
using Fellow.Epi.Localization.Infrastructure.Editor.Content.Definition;

namespace Fellow.Epi.Localization.Infrastructure.Initialization
{
	[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
	public class ExternalTranslationContentProviderInitialization : IInitializableModule
	{
		private bool _isInitialized;

		public void Initialize(InitializationEngine context)
		{
			if (!this._isInitialized)
			{
				var newsContentProvider = context.Locate.Advanced.GetInstance<ExternalTranslationContentProvider>();

				// add configuration settings for entry point and capabilites
				var providerValues = new NameValueCollection {
					{ ContentProviderElement.EntryPointString, newsContentProvider.GetEntryPoint(TranslationDataRepositoryDescriptor.RepositoryKey).ToString() },
					{ ContentProviderElement.CapabilitiesString, "Edit,Search" }
				};

				// initialize and register the provider
				newsContentProvider.Initialize(TranslationDataRepositoryDescriptor.RepositoryKey, providerValues);
				
				var providerManager = context.Locate.Advanced.GetInstance<IContentProviderManager>();
				providerManager.ProviderMap.AddProvider(newsContentProvider);
			}

			this._isInitialized = true;
		}

		public void Uninitialize(InitializationEngine context) { }

		public void Preload(string[] parameters) { }
	}
}
