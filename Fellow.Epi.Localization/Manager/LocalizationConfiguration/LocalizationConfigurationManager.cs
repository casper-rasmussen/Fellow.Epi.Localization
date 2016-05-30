using System;

namespace Fellow.Epi.Localization.Manager.LocalizationConfiguration
{
	public class LocalizationConfigurationManager : ILocalizationConfigurationManager
	{
		public string DefaultResourceArea
		{
			get { return "Default"; }
			
		}

		public TimeSpan TranslationCachingTimeSpan
		{
			get { return TimeSpan.FromMinutes(2); }
		}

		public TimeSpan ResourceCachingTimeSpan
		{
			get { return TimeSpan.FromMinutes(4); }
			
		}
	}
}
