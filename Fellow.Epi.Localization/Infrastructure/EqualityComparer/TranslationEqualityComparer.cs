using System;
using System.Collections.Generic;
using Fellow.Epi.Localization.Manager.Translation.Entities;

namespace Fellow.Epi.Localization.Infrastructure.EqualityComparer
{
	class TranslationEqualityComparer : IEqualityComparer<ITranslation>
	{
		public bool Equals(ITranslation x, ITranslation y)
		{
			return x.Key.Equals(y.Key, StringComparison.InvariantCultureIgnoreCase);
		}

		public int GetHashCode(ITranslation obj)
		{
			return obj.Key.ToLowerInvariant().GetHashCode();
		}
	}
}
