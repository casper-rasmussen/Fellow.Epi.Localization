using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace Fellow.Epi.Localization.Models.Content
{
	[AdministrationSettings(
		  CodeOnly = true,
		  GroupName = LocalizationGroupNames.TranslationTypeGroupName)]
	[ContentType(
		   GUID = "14bbf4a1-cd38-47f0-a550-1028cc989c6f",
		   AvailableInEditMode = false,
		   GroupName = LocalizationGroupNames.TranslationTypeGroupName,
		   DisplayName = "Default Translaton")]
	public class DefaultTranslationType : BasicContent, ILocalizable
	{
		[Display(Name = "Translation")]
		[CultureSpecific(true)]
		public virtual string Translation { get; set; }

		[ScaffoldColumn(false)]
		public virtual string Key { get; set; }

		public IEnumerable<CultureInfo> ExistingLanguages { get; set; }

		public CultureInfo MasterLanguage { get; set; }

		public CultureInfo Language { get; set; }
	}
}
