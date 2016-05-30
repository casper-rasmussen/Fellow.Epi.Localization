using System;
using System.Globalization;
using EPiServer.Framework.Cache;
using Fellow.Epi.Localization.Manager.LocalizationConfiguration;

namespace Fellow.Epi.Localization.Repository.Translation
{
	class TranslationRepositoryCachingProxy : TranslationRepository
	{
		private readonly ISynchronizedObjectInstanceCache _cache;
		private readonly ILocalizationConfigurationManager _localizationConfigurationManager;

		public TranslationRepositoryCachingProxy(ISynchronizedObjectInstanceCache cache, ILocalizationConfigurationManager localizationConfigurationManager)
			: base()
		{
			this._cache = cache;
			this._localizationConfigurationManager = localizationConfigurationManager;
		}

		public override string Get(string key, CultureInfo culture)
		{
			string cacheKey = this.GetCacheKey(key, culture);

			string translation = this._cache.Get(cacheKey) as string;

			if (!String.IsNullOrWhiteSpace(translation))
				return translation;

			//Resolve via underlying manager
			translation = base.Get(key, culture);

			if (String.IsNullOrWhiteSpace(translation))
				return translation;

			this._cache.Insert(cacheKey, translation, new CacheEvictionPolicy(this._localizationConfigurationManager.TranslationCachingTimeSpan, CacheTimeoutType.Sliding));

			return translation;
		}

		public override void Add(string key, string translation, CultureInfo culture)
		{
			base.Add(key, translation, culture);
			
			string cacheKey = this.GetCacheKey(key, culture);

			//Purge cache
			this._cache.Remove(cacheKey);
		}

		public override void Remove(string key, CultureInfo culture)
		{
			base.Remove(key, culture);

			string cacheKey = this.GetCacheKey(key, culture);

			//Purge cache
			this._cache.Remove(cacheKey);
		}

		private string GetCacheKey(string key, CultureInfo culture)
		{
			return String.Format("TranslationRepository_{0}_{1}", key, culture.LCID);
		}
	}
}
