using System.Globalization;
using Fellow.Epi.Localization.Repository.Translation.Exceptions;

namespace Fellow.Epi.Localization.Repository.Translation
{
	public interface ITranslationRepository
	{
		/// <exception cref="TranslationNotFoundException">Thrown when Translation was not found</exception>
		string Get(string key, CultureInfo culture);

		void Remove(string key, CultureInfo culture);

		void Add(string key, string translation, CultureInfo culture);
	}
}
