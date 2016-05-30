using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Fellow.Epi.Localization.Manager.Convention;
using Fellow.Epi.Localization.Repository.Resource;
using Fellow.Epi.Localization.Repository.Resource.Entity;

namespace Fellow.Epi.Localization.Manager.Resource
{
	class ResourceManager : IResourceManager
	{
		private readonly IEnumerable<IResourceRepository> _resourceRepositories;
		private readonly IConventionManager _conventionManager;

		public ResourceManager(IEnumerable<IResourceRepository> resourceRepositories, IConventionManager conventionManager)
		{
			this._resourceRepositories = resourceRepositories;
			this._conventionManager = conventionManager;
		}

		public virtual IEnumerable<IResource> GetAll(CultureInfo culture)
		{
			return this._resourceRepositories
				.SelectMany(repo => repo.GetAll(culture))
				.Where(r => this._conventionManager.IsIncluded(r.Key)).ToList();
		}

		public virtual bool TryGet(string key, CultureInfo culture, out IResource resource)
		{
			resource = default(IResource);

			foreach (IResourceRepository resourceRepository in _resourceRepositories)
			{
				bool found = resourceRepository.Exists(key, culture);

				if (!found)
					continue;

				resource = resourceRepository.Get(key, culture);

				bool allowed = this._conventionManager.IsIncluded(resource.Key);

				if (!allowed)
				{
					resource = default(IResource);

					continue;
				}

				return resource != null;
			}

			return false;
		}
	}
}
