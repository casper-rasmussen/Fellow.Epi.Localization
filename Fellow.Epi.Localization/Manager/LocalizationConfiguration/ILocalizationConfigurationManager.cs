using System;

namespace Fellow.Epi.Localization.Manager.LocalizationConfiguration
{
	public interface ILocalizationConfigurationManager
	{
		string DefaultResourceArea { get; }

		TimeSpan TranslationCachingTimeSpan { get; }

		TimeSpan ResourceCachingTimeSpan { get; }
	}
}
