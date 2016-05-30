using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Globalization;

namespace Fellow.Epi.Localization.Manager.Convention
{
	class ConventionManager : IConventionManager
	{
		private static readonly IList<string> Areas = new List<string>();
		
		public void IncludeArea(string area)
		{
			Areas.Add(area);
		}

		public bool IsIncluded(string resourceKey)
		{
			TextInfo textInfo = ContentLanguage.PreferredCulture.TextInfo;

			IEnumerable<string> segments = resourceKey
				.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(segment => textInfo.ToTitleCase(segment));

			//First segment is the area
			string area = segments.FirstOrDefault();

			bool byConvention = Areas.Contains(area, StringComparer.InvariantCultureIgnoreCase);

			return byConvention;
		}
	}
}
