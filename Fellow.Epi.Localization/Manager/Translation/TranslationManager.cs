using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Globalization;
using Fellow.Epi.Localization.Infrastructure.EqualityComparer;
using Fellow.Epi.Localization.Manager.Resource;
using Fellow.Epi.Localization.Manager.Translation.Entities;
using Fellow.Epi.Localization.Repository.Resource.Entity;
using Fellow.Epi.Localization.Repository.Translation;
using Fellow.Epi.Localization.Repository.Translation.Exceptions;

namespace Fellow.Epi.Localization.Manager.Translation
{
	class TranslationManager : ITranslationManager
	{
		private readonly IResourceManager _resourceManager;
		private readonly ITranslationRepository _translationRepository;

		public TranslationManager(IResourceManager resourceManager, ITranslationRepository translationRepository)
		{
			this._resourceManager = resourceManager;
			this._translationRepository = translationRepository;
		}

		public virtual bool TryGet(string key, CultureInfo culture, out ITranslation translation)
		{
			translation = default(ITranslation);

			IResource resource = default(IResource);

			//Lets get the default resource
			bool found = this._resourceManager.TryGet(key, culture, out resource);

			//If it does not exist, then don't allow it.
			if (!found)
			{
				//Try for our Master Language
				found = this._resourceManager.TryGet(key, ContentLanguage.Instance.FinalFallbackCulture, out resource);

				if (!found)
					return false;
			}

			//Try and get the translation from our local source (overrides)
			bool foundLocal = this.TryGetLocalTranslation(key, culture, out translation);

			if (foundLocal)
				return true;
			
			//Get the default translation based on culture or master
			translation = GetTranslation(key, resource.DefaultValue, culture);

			return true;

		}

		public virtual IEnumerable<ITranslation> GetAll(CultureInfo culture)
		{
			//Get all available resources
			IEnumerable<IResource> items = this._resourceManager.GetAll(culture);
			
			//Convert
			IList<ITranslation> translations = items
							.Select(resource => this.GetTranslation(resource.Key, resource.DefaultValue, culture))
							.ToList();

			//If we aren't on our master language
			if (!culture.Equals(ContentLanguage.Instance.FinalFallbackCulture))
			{
				//Get all for our master culture in case language has no translation for all keys
				IEnumerable<ITranslation> fallback = this.GetAll(ContentLanguage.Instance.FinalFallbackCulture);

				translations = translations.Union(fallback, new TranslationEqualityComparer()).ToList();
			}

			return translations;
		}

		private ITranslation GetTranslation(string key, string translation, CultureInfo culture)
		{
			if (String.IsNullOrWhiteSpace(translation))
				translation = String.Empty;

			TextInfo textInfo = culture.TextInfo;
			
			//Get the segments and convert to Title Case
			IEnumerable<string> segments = key
				.Split(new char[] {'/'}, StringSplitOptions.RemoveEmptyEntries)
				.Select(segment => textInfo.ToTitleCase(segment));
			
			//First segment is the context
			string area = segments.FirstOrDefault();
			
			//Skip the first segment and combine the rest 
			string name = string.Join(" > ", segments.Skip(1));

			return new Entities.Translation()
			{
				Key = key,
				Name = name,
				TranslationText = translation,
				Area = textInfo.ToTitleCase(area)
			};
		}

		public virtual void Update(string translation, string key, CultureInfo culture)
		{
			this._translationRepository.Remove(key, culture);

			//If empty, then remove and fallback to default
			if (String.IsNullOrWhiteSpace(translation))
				return;
			
			IResource resource;

			//Lets get the default
			bool found = this._resourceManager.TryGet(key, culture, out resource);

			if (found)
			{
				//Compare if the text is the same as our default. If yes, then skip saving.
				if (resource.DefaultValue.Equals(translation))
					return;
			}

			this._translationRepository.Add(key, translation, culture);
		}

		private bool TryGetLocalTranslation(string key, CultureInfo culture, out ITranslation localTranslation)
		{
			try
			{
				string translation = this._translationRepository.Get(key, culture);

				//Convert to Translation
				localTranslation = this.GetTranslation(key, translation, culture);
				
				return localTranslation != null;
			}
			catch (TranslationNotFoundException e)
			{
				localTranslation = default(ITranslation);
				return false;
			}
		}
	}
}
