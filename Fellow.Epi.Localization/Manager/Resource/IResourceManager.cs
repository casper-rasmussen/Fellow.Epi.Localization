using System.Collections.Generic;
using System.Globalization;
using Fellow.Epi.Localization.Repository.Resource.Entity;

namespace Fellow.Epi.Localization.Manager.Resource
{
	public interface IResourceManager
	{
		bool TryGet(string key, CultureInfo culture, out IResource resource);

		IEnumerable<IResource> GetAll(CultureInfo culture);
	}
}
