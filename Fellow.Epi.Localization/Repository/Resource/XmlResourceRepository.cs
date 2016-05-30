using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web.Configuration;
using EPiServer.Framework.Configuration;
using EPiServer.Framework.Localization;
using EPiServer.Framework.Localization.XmlResources;
using Fellow.Epi.Localization.Repository.Resource.Entity;

namespace Fellow.Epi.Localization.Repository.Resource
{
	class XmlResourceRepository : IResourceRepository
	{
		private  LocalizationProvider _xmlProvider;
		private ResourceKeyHandler _resourceKeyHandler;

		public XmlResourceRepository(EPiServerFrameworkSection configurationSection, ResourceKeyHandler resourceKeyHandler)
		{
			IEnumerable<ProviderSettings> settings = configurationSection.Localization.Providers.Cast<ProviderSettings>();

			foreach (ProviderSettings providerSetting in settings)
			{
				LocalizationProvider provider = ProvidersHelper.InstantiateProvider(providerSetting, typeof(LocalizationProvider)) as XmlLocalizationProvider;

				//Set the provider
				if (provider != null)
					this._xmlProvider = provider;
			}

			this._resourceKeyHandler = resourceKeyHandler;
		}

		public IResource Get(string key, CultureInfo culture)
		{
			string defaultValue = this._xmlProvider.GetString(key, this._resourceKeyHandler.NormalizeKey(key), culture);

			if (defaultValue == null)
				return null;

			return new Entity.Resource() { DefaultValue = defaultValue, Key = key };
		}

		public IEnumerable<IResource> GetAll(CultureInfo culture)
		{
			IEnumerable<ResourceItem> items = this._xmlProvider.GetAllStrings(String.Empty, this._resourceKeyHandler.NormalizeKey(String.Empty), culture);

			return items.Select(item => new Entity.Resource() { DefaultValue = item.Value, Key = item.Key });
		}


		public bool Exists(string key, CultureInfo culture)
		{
			return this.Get(key, culture) != null;
		}
	}
}
