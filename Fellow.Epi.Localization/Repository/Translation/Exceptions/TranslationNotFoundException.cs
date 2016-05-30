using System;
using System.Globalization;

namespace Fellow.Epi.Localization.Repository.Translation.Exceptions
{
	public class TranslationNotFoundException : Exception
	{
		public TranslationNotFoundException(string key, CultureInfo culture)
			: base(String.Format("Translation was not found for {0} in {1}", key, culture.Name))
		{
			
		}
	}
}
