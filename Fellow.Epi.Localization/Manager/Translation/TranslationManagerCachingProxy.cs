using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer.Framework.Cache;
using Fellow.Epi.Localization.Manager.LocalizationConfiguration;
using Fellow.Epi.Localization.Manager.Resource;
using Fellow.Epi.Localization.Manager.Translation.Entities;
using Fellow.Epi.Localization.Repository.Translation;

namespace Fellow.Epi.Localization.Manager.Translation
{
	class TranslationManagerCachingProxy : TranslationManager
	{
		private readonly ISynchronizedObjectInstanceCache _cache;
		private readonly ILocalizationConfigurationManager _localizationConfigurationManager;

		public TranslationManagerCachingProxy(ISynchronizedObjectInstanceCache cache, ILocalizationConfigurationManager localizationConfigurationManager, IResourceManager resourceManager, ITranslationRepository translationRepository)
			: base(resourceManager, translationRepository)
		{
			this._cache = cache;
			this._localizationConfigurationManager = localizationConfigurationManager;
		}

		public override bool TryGet(string key, CultureInfo culture, out ITranslation translation)
		{
			string cacheKey = this.GetCacheKey(key, culture);

			translation = this._cache.Get(cacheKey) as ITranslation;

			if (translation != null)
				return true;

			//Resolve via underlying manager
			bool found = base.TryGet(key, culture, out translation);

			if (!found)
				return false;

			this._cache.Insert(cacheKey, translation, new CacheEvictionPolicy(this._localizationConfigurationManager.TranslationCachingTimeSpan, CacheTimeoutType.Sliding));

			return true;
		}

		public override void Update(string translation, string key, CultureInfo culture)
		{
			base.Update(translation, key, culture);

			string cacheKey = this.GetCacheKey(key, culture);

			//Purge cache
			this._cache.Remove(cacheKey);

			cacheKey = this.GetCacheKey("GetAll", culture);

			this._cache.Remove(cacheKey);
		}

		public override IEnumerable<ITranslation> GetAll(CultureInfo culture)
		{
			string cacheKey = this.GetCacheKey("GetAll", culture);

			IEnumerable<ITranslation> translations = this._cache.Get(cacheKey) as IEnumerable<ITranslation>;

			if (translations != null)
				return translations;

			//Resolve via underlying manager
			translations = base.GetAll(culture);
			
			this._cache.Insert(cacheKey, translations, new CacheEvictionPolicy(this._localizationConfigurationManager.TranslationCachingTimeSpan, CacheTimeoutType.Sliding));

			return translations;
		}

		private string GetCacheKey(string key, CultureInfo culture)
		{
			return String.Format("TranslationManager_{0}_{1}", key, culture.LCID);
		}
	}
}
