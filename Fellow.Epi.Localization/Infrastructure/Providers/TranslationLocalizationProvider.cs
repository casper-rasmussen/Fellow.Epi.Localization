using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer.Framework.Localization;
using EPiServer.Framework.Localization.XmlResources;
using EPiServer.ServiceLocation;
using Fellow.Epi.Localization.Manager.Convention;
using Fellow.Epi.Localization.Repository.Translation;
using Fellow.Epi.Localization.Repository.Translation.Exceptions;

namespace Fellow.Epi.Localization.Infrastructure.Providers
{
	public class TranslationLocalizationProvider : FileXmlLocalizationProvider
	{
		private readonly ITranslationRepository _translationRepository;
		private readonly IConventionManager _conventionManager;

		public TranslationLocalizationProvider()
		{
			this._translationRepository = ServiceLocator.Current.GetInstance<ITranslationRepository>();
			this._conventionManager = ServiceLocator.Current.GetInstance<IConventionManager>();
		}

		public override string GetString(string originalKey, string[] normalizedKey, CultureInfo culture)
		{
			if (!this._conventionManager.IsIncluded(originalKey))
				return null;
			
			//Lets get the default resource
			string cultureString = base.GetString(originalKey, normalizedKey, culture);

			string value;

			//Try and get the translation from our local source (overrides)
			bool foundLocal = this.TryGetLocalTranslation(originalKey, culture, out value);

			if (foundLocal)
				return value; //Found an override

			return cultureString;
		}
		
		public override IEnumerable<ResourceItem> GetAllStrings(string originalKey, string[] normalizedKey, CultureInfo culture)
		{
			return new List<ResourceItem>();
		}

		private bool TryGetLocalTranslation(string key, CultureInfo culture, out string value)
		{
			try
			{
				value = this._translationRepository.Get(key, culture);

				return !String.IsNullOrWhiteSpace(value);
			}
			catch (TranslationNotFoundException e)
			{
				value = null;
				return false;
			}
		}
	}
}
