using System.Collections.Generic;
using System.Globalization;
using Fellow.Epi.Localization.Manager.Translation.Entities;

namespace Fellow.Epi.Localization.Manager.Translation
{
	public interface ITranslationManager
	{
		bool TryGet(string key, CultureInfo culture, out ITranslation translation);

		IEnumerable<ITranslation> GetAll(CultureInfo culture);

		void Update(string key, string translation, CultureInfo culture);
	}
}
