using System;
using System.Collections.Generic;
using System.Globalization;
using EPiServer.Framework.Cache;
using Fellow.Epi.Localization.Manager.Convention;
using Fellow.Epi.Localization.Manager.LocalizationConfiguration;
using Fellow.Epi.Localization.Repository.Resource;
using Fellow.Epi.Localization.Repository.Resource.Entity;

namespace Fellow.Epi.Localization.Manager.Resource
{
	class ResourceManagerCachingProxy : ResourceManager
	{
		private readonly ISynchronizedObjectInstanceCache _cache;
		private readonly ILocalizationConfigurationManager _localizationConfigurationManager;

		public ResourceManagerCachingProxy(IEnumerable<IResourceRepository> resourceRepositories, IConventionManager conventionManager, ISynchronizedObjectInstanceCache cache, ILocalizationConfigurationManager localizationConfigurationManager)
			: base(resourceRepositories, conventionManager)
		{
			this._cache = cache;
			this._localizationConfigurationManager = localizationConfigurationManager;
		}

		public override IEnumerable<IResource> GetAll(CultureInfo culture)
		{
			string cacheKey = this.GetCacheKey("GetAll", culture);

			IEnumerable<IResource> resources = this._cache.Get(cacheKey) as IEnumerable<IResource>;

			if (resources != null)
				return resources;

			//Resolve via underlying manager
			resources = base.GetAll(culture);

			this._cache.Insert(cacheKey, resources, new CacheEvictionPolicy(this._localizationConfigurationManager.ResourceCachingTimeSpan, CacheTimeoutType.Sliding));

			return resources;
		}

		public override bool TryGet(string key, CultureInfo culture, out IResource resource)
		{
			string cacheKey = this.GetCacheKey(key, culture);

			resource = this._cache.Get(cacheKey) as IResource;

			if (resource != null)
				return true;

			//Resolve via underlying manager
			bool found = base.TryGet(key, culture, out resource);

			if (!found)
				return false;

			this._cache.Insert(cacheKey, resource, new CacheEvictionPolicy(this._localizationConfigurationManager.ResourceCachingTimeSpan, CacheTimeoutType.Sliding));

			return true;
		}

		private string GetCacheKey(string key, CultureInfo culture)
		{
			return String.Format("ResourceManager{0}_{1}", key, culture.LCID);
		}
	}
}
