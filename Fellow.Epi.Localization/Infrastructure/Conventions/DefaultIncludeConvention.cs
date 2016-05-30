using Fellow.Epi.Localization.Manager.Convention;
using Fellow.Epi.Localization.Manager.LocalizationConfiguration;

namespace Fellow.Epi.Localization.Infrastructure.Conventions
{
	class DefaultIncludeConvention : IIncludeConvention
	{
		private readonly ILocalizationConfigurationManager _localizationConfigurationManager;

		public DefaultIncludeConvention(ILocalizationConfigurationManager localizationConfigurationManager)
		{
			this._localizationConfigurationManager = localizationConfigurationManager;
		}

		public void Apply(IConventionManager conventionManager)
		{
			string defaultArea = this._localizationConfigurationManager.DefaultResourceArea;

			conventionManager.IncludeArea(defaultArea);
		}
	}
}
