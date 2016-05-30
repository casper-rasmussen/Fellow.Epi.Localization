using System.Collections.Generic;
using System.Globalization;
using Fellow.Epi.Localization.Repository.Resource.Entity;

namespace Fellow.Epi.Localization.Repository.Resource
{
	public interface IResourceRepository
	{
		bool Exists(string key, CultureInfo culture);

		IResource Get(string key, CultureInfo culture);

		IEnumerable<IResource> GetAll(CultureInfo culture);
	}
}
